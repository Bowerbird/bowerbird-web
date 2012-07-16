/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceCreateCommandHandler : ICommandHandler<MediaResourceCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IImageService _imageService;
        private readonly IVideoService _videoService;

        #endregion

        #region Constructors

        public MediaResourceCreateCommandHandler(
            IDocumentSession documentSession,
            IImageService imageService,
            IVideoService videoService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(videoService, "videoService");
            Check.RequireNotNull(imageService, "imageService");

            _documentSession = documentSession;
            _videoService = videoService;
            _imageService = imageService;
        }

        #endregion

        #region Methods

        public void Handle(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var mediaType = command.MediaType.ToLower();
            var user = _documentSession.Load<User>(command.UserId);
            var mediaResource = new MediaResource(
                            mediaType,
                            user,
                            command.UploadedOn,
                            command.Key);

            _documentSession.Store(mediaResource);
            // looks like we need to call savechanges to actually get an id back
            _documentSession.SaveChanges();

            switch (mediaType)
            {
                case "image":
                    {
                        _imageService.Save(command, mediaResource);
                    }
                    break;

                case "video":
                    {
                        _videoService.Save(command, mediaResource);
                    }
                    break;
            }

            _documentSession.Store(mediaResource);

            mediaResource.FireCreatedEvent(user);
        }

        #endregion
    }
}