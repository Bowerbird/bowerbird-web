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
using Raven.Client;
using Bowerbird.Core.Config;

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

            if (!GetMediaService(command).Save(command, out failureReason)) 
            {
                var user = _documentSession.Load<User>(command.UserId);
                _messageBus.Publish(new MediaResourceCreateFailedEvent(user, command.Key, failureReason, user));
            }
        }

        private IMediaService GetMediaService(MediaResourceCreateCommand command)
        {
            if (command.Type == "file")
            {
                var mediaTypeInfo = MediaTypeUtility.GetMediaTypeInfoForFile(command.FileStream);

                if (mediaTypeInfo.MediaType == "image")
                {
                    return _mediaServiceFactory.CreateImageService();
                }
                else if (mediaTypeInfo.MediaType == "audio")
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
                else if (command.VideoProviderName.ToLower() == "vimeo")
                {
                    return _mediaServiceFactory.CreateVimeoVideoService();
                }
            }

            throw new ArgumentException(string.Format("A media service could not be found for the specified type '{0}'.", command.Type));
        }

        #endregion
    }
}