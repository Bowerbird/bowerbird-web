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

        //public MediaResource HandleReturn(MediaResourceCreateCommand command)
        //{
        //    Check.RequireNotNull(command, "command");

        //    ImageUtility image = null;

        //    try
        //    {
        //        string mediaType = DetemineMediaType(command);

        //        var mediaResource = new MediaResource(
        //                        mediaType,
        //                        _documentSession.Load<User>(command.UserId),
        //                        command.UploadedOn,
        //                        command.Key);

        //        _documentSession.Store(mediaResource);

        //        switch (mediaType)
        //        {
        //            case "image":
        //                ImageDimensions imageDimensions;

        //                IDictionary<string, object> exifData;
        //                var imageCreationTasks = new List<ImageCreationTask>();

        //                image = ImageUtility
        //                    .Load(command.Stream)
        //                    .GetExifData(out exifData)
        //                    .GetImageDimensions(out imageDimensions);

        //                MakeOriginalImageMediaResourceFile(mediaResource, imageCreationTasks, command.OriginalFileName, command.Stream.Length, imageDimensions, exifData);

        //                if (command.Usage == "observation")
        //                {
        //                    MakeObservationImageMediaResourceFiles(mediaResource, imageCreationTasks);
        //                }
        //                else if (command.Usage == "post")
        //                {
        //                    MakePostImageMediaResourceFiles(mediaResource, imageCreationTasks);
        //                }
        //                else if (command.Usage == "user")
        //                {
        //                    MakeUserImageMediaResourceFiles(mediaResource, imageCreationTasks);
        //                }
        //                else if (command.Usage == "group")
        //                {
        //                    MakeGroupImageMediaResourceFiles(mediaResource, imageCreationTasks);
        //                }
        //                else
        //                {
        //                    MakeOtherImageMediaResourceFiles(mediaResource, imageCreationTasks);
        //                }

        //                SaveImages(image, mediaResource, imageCreationTasks);
        //                break;
        //        }

        //        _documentSession.Store(mediaResource);

        //        return mediaResource;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (image != null)
        //            image.Cleanup();

        //        throw ex;
        //    }
        //}

        public void Handle(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var mediaType = command.MediaType;
            var user = _documentSession.Load<User>(command.UserId);
            var mediaResource = new MediaResource(
                            mediaType,
                            user,
                            command.UploadedOn,
                            command.Key);

            _documentSession.Store(mediaResource);

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