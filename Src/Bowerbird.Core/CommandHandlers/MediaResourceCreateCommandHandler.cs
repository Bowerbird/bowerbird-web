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
using Bowerbird.Core.Infrastructure;
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
        private readonly IMessageBus _messageBus;

        #endregion

        #region Constructors

        public MediaResourceCreateCommandHandler(
            IDocumentSession documentSession,
            IMediaServiceFactory mediaServiceFactory,
            IMessageBus messageBus
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaServiceFactory, "mediaServiceFactory");
            Check.RequireNotNull(messageBus, "messageBus");

            _documentSession = documentSession;
            _mediaServiceFactory = mediaServiceFactory;
            _messageBus = messageBus;
        }

        #endregion

        #region Methods

        public void Handle(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            string failureReason;
            MediaResource mediaResource;

            var createdByUser = _documentSession.Load<User>(command.UserId);

            if (GetMediaService(command).Save(command, createdByUser, out failureReason, out mediaResource))
            {
                _messageBus.Publish(new DomainModelCreatedEvent<MediaResource>(mediaResource, createdByUser, mediaResource));

                _documentSession.Store(mediaResource);
                _documentSession.SaveChanges();
            }
            else
            {
                _messageBus.Publish(new MediaResourceCreateFailedEvent(createdByUser, command.Key, failureReason, createdByUser));
            }
        }

        private IMediaService GetMediaService(MediaResourceCreateCommand command)
        {
            if (command.Type == "file")
            {
                var mediaType = MediaTypeUtility.GetMediaTypeInfoForMimeType(command.FileMimeType).MediaType;

                if (mediaType == "image")
                {
                    return _mediaServiceFactory.CreateImageService();
                }
                if (mediaType == "audio")
                {
                    return _mediaServiceFactory.CreateAudioService();
                }
            }
            else if (command.Type == "externalvideo")
            {
                if (command.VideoProviderName.ToLower() == "youtube")
                {
                    return _mediaServiceFactory.CreateYouTubeVideoService();
                }
                if (command.VideoProviderName.ToLower() == "vimeo")
                {
                    return _mediaServiceFactory.CreateVimeoVideoService();
                }
            }

            throw new ArgumentException(string.Format("A media service could not be found for the specified type '{0}'.", command.Type));
        }

        #endregion
    }
}