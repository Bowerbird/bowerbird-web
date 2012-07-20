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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;
using Bowerbird.Core.Services;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceCreateCommandHandler : ICommandHandler<MediaResourceCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IImageService _imageService;
        private readonly IMediaServiceFactory _mediaServiceFactory;

        #endregion

        #region Constructors

        public MediaResourceCreateCommandHandler(
            IDocumentSession documentSession,
            IImageService imageService,
            IMediaServiceFactory mediaServiceFactory
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaServiceFactory, "mediaServiceFactory");
            Check.RequireNotNull(imageService, "imageService");

            _documentSession = documentSession;
            _mediaServiceFactory = mediaServiceFactory;
            _imageService = imageService;
        }

        #endregion

        #region Methods

        public void Handle(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var user = _documentSession.Load<User>(command.UserId);

            var mediaResource = new MediaResource(
                command.MediaType.ToLower(),
                user,
                command.UploadedOn,
                command.Key);

            _documentSession.Store(mediaResource);
            
            // Looks like we need to call savechanges to actually get an id back
            _documentSession.SaveChanges();

            string failureReason = string.Empty;

            if (GetMediaService(command).Save(command, mediaResource, out failureReason))
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

        private IMediaService GetMediaService(MediaResourceCreateCommand command)
        {
            string mediaType = command.MediaType.ToLower();

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

            throw new ArgumentException(string.Format("The specified mediatype '{0}' is not supported.", mediaType));
        }

        #endregion
    }
}