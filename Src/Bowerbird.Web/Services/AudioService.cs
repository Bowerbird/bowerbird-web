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
using System.Collections.Generic;
using System.IO;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using NLog;
using Raven.Client;
using System.Linq;
using Bowerbird.Core.Factories;
using TagLib;
using File = System.IO.File;

namespace Bowerbird.Web.Services
{
    public class AudioService : IAudioService
    {
        #region Fields

        private Logger _logger = LogManager.GetLogger("AudioService");

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathFactory _mediaFilePathFactory;

        /// <summary>
        /// Supported audio formats:
        /// MP3: audio/mpeg
        /// MP4: audio/mp4 
        /// OGG: audio/ogg 
        /// WebM: audio/webm
        /// WAV: audio/wav
        /// </summary>
        private readonly IEnumerable<string> _supportedMimeTypes = new[] { "audio/mpeg", "audio/mp3", "audio/wav" };

        #endregion

        #region Constructors

        public AudioService(
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathFactory mediaFilePathFactory)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");

            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathFactory = mediaFilePathFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, MediaResource mediaResource, out string failureReason)
        {
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).AudioServiceStatus)
            {
                failureReason = "Audio files cannot be uploaded at the moment. Please try again later.";
                return false;
            }

            try
            {
                var audioFile = TagLib.File.Create(new FileAbstraction(command.Stream, command.OriginalFileName), command.MimeType, ReadStyle.None);

                if (audioFile.PossiblyCorrupt || !IsSupportedMimeType(audioFile.MimeType))
                {
                    failureReason = "The file is corrupted or not a valid audio file and could not be saved. Please check the file and try again.";
                    return false;
                }

                string extension = GetExtension(audioFile.MimeType);

                MakeAudioMediaResourceFiles(mediaResource, command, audioFile, extension);

                string filePath = _mediaFilePathFactory.MakeMediaFilePath(mediaResource.Id, "audio", "Original", extension);

                using (var fileStream = File.Create(filePath))
                {
                    command.Stream.Seek(0, SeekOrigin.Begin);
                    command.Stream.CopyTo(fileStream);
                }

            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving audio", exception);

                failureReason = "The file is corrupted or not a valid audio file and could not be saved. Please check the file and try again.";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }


        private bool IsSupportedMimeType(string mimeType)
        {
            return _supportedMimeTypes.Any(x => x == mimeType.ToLower());
        }

        private string GetExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "audio/mpeg":
                case "audio/mp3": // Chrome (as of v20) returns this non-standard mimetype
                    return "mp3";
                case "audio/wav":
                    return "wav";
            }

            return string.Empty;
        }

        private void MakeAudioMediaResourceFiles(MediaResource mediaResource, MediaResourceCreateCommand command, TagLib.File audioFile, string extension)
        {
            string fileName = _mediaFilePathFactory.MakeMediaFileName(mediaResource.Id, "Original", extension);
            string relativeUri = _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, "audio", "Original", extension);

            dynamic original = mediaResource.AddAudioFile("Original", command.OriginalFileName, relativeUri, audioFile.MimeType, extension);

            original.Title = audioFile.Tag.Title;
            original.Copyright = audioFile.Tag.Copyright;
            original.Comment = audioFile.Tag.Comment;

            mediaResource.AddAudioFile("Square42", fileName, relativeUri, audioFile.MimeType, extension);
            mediaResource.AddAudioFile("Square100", fileName, relativeUri, audioFile.MimeType, extension);
            mediaResource.AddAudioFile("Square200", fileName, relativeUri, audioFile.MimeType, extension);
            mediaResource.AddAudioFile("Full480", fileName, relativeUri, audioFile.MimeType, extension);
            mediaResource.AddAudioFile("Full768", fileName, relativeUri, audioFile.MimeType, extension);
            mediaResource.AddAudioFile("Full1024", fileName, relativeUri, audioFile.MimeType, extension);
        }

        #endregion
    }

    public class FileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly Stream _stream;

        private readonly string _fileName;

        public FileAbstraction(Stream stream, string fileName)
        {
            _stream = stream;
            _fileName = fileName;
        }

        public string Name
        {
            get { return _fileName; }
        }

        public Stream ReadStream
        {
            get { return _stream; }
        }

        public Stream WriteStream
        {
            get { throw new NotImplementedException(); }
        }

        public void CloseStream(Stream stream)
        {
        }

    }
}