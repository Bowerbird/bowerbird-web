/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.IO;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;
using Bowerbird.Core.Services;
using Bowerbird.Core.Utilities;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceCreateCommandHandler : ICommandHandler<MediaResourceCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaServiceFactory _mediaServiceFactory;

        #endregion

        #region Constructors

        public MediaResourceCreateCommandHandler(
            IDocumentSession documentSession,
            IMediaServiceFactory mediaServiceFactory
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaServiceFactory, "mediaServiceFactory");

            _documentSession = documentSession;
            _mediaServiceFactory = mediaServiceFactory;
        }

        #endregion

        #region Methods

        public void Handle(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            string mediaType;
            var user = _documentSession.Load<User>(command.UserId);

            if (IsKnownMediaType(command, out mediaType))
            {
                var mediaResource = new MediaResource(
                    mediaType,
                    user,
                    command.UploadedOn,
                    command.Key);

                _documentSession.Store(mediaResource);

                // Looks like we need to call savechanges to actually get an id back
                _documentSession.SaveChanges();

                string failureReason;

                if (GetMediaService(mediaType, command).Save(command, mediaResource, out failureReason))
                {
                    _documentSession.Store(mediaResource);
                    mediaResource.FireCreatedEvent(user);
                }
                else
                {
                    _documentSession.Delete(mediaResource);
                    EventProcessor.Raise(new MediaResourceCreateFailedEvent(user, command.Key, failureReason, user));
                }
            }
            else
            {
                EventProcessor.Raise(new MediaResourceCreateFailedEvent(user, command.Key, "The uploaded file is not an accepted file type.", user));
            }
        }

        /// <summary>
        /// Performs an exhaustive search for hints about what media type the media resource is
        /// </summary>
        private bool IsKnownMediaType(MediaResourceCreateCommand command, out string mediaType)
        {
            mediaType = null;

            // Check if a media types is explicitly specified
            if (!string.IsNullOrWhiteSpace(command.MediaType))
            {
                mediaType = command.MediaType.ToLower();
                return true;
            }

            // Check if a mime type is explicitly specified
            if (!string.IsNullOrWhiteSpace(command.MimeType))
            {
                switch (command.MimeType.ToLower())
                {
                    case "audio/mpeg":
                    case "audio/wav":
                        mediaType = "audio";
                        return true;
                    case "image/jpeg":
                        mediaType = "image";
                        return true;
                }
            }

            // Check if the original filename (if provided) contains a file extension
            if (!string.IsNullOrWhiteSpace(command.OriginalFileName) && command.OriginalFileName.Contains("."))
            {
                var extension = command.OriginalFileName.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries).Last().ToLower();

                switch (extension)
                {
                    case "jpeg":
                    case "jpg":
                        mediaType = "image";
                        return true;
                    case "mp3":
                    case "wav":
                        mediaType = "audio";
                        return true;
                }
            }

            // Check if the binary data contains header info
            if (command.Stream != null)
            {
                if (IsJpeg(command.Stream))
                {
                    mediaType = "image";
                    return true;
                }
            }

            return false;
        }

        private bool IsJpeg(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(stream))
            {
                UInt16 soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                UInt16 jfif = br.ReadUInt16(); // JFIF marker (FFE0)

                return soi == 0xd8ff && jfif == 0xe0ff;
            }
        }

        private IMediaService GetMediaService(string mediaType, MediaResourceCreateCommand command)
        {
            if (mediaType == "image")
            {
                return _mediaServiceFactory.CreateImageService();
            } 
            else if (mediaType == "video") 
            {
                if (command.VideoProviderName.ToLower() == "youtube")
                {
                    return _mediaServiceFactory.CreateYouTubeVideoService();
                }
                else if (command.VideoProviderName.ToLower() == "vimeo")
                {
                    return _mediaServiceFactory.CreateVimeoVideoService();
                }
            }
            else if (mediaType == "audio")
            {
                return _mediaServiceFactory.CreateAudioService();
            }

            throw new ArgumentException(string.Format("The specified mediatype '{0}' is not supported.", mediaType));
        }

        #endregion
    }
}