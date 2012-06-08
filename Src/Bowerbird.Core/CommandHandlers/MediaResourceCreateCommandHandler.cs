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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Commands;
using Raven.Client;
using Bowerbird.Core.ImageUtilities;
using Bowerbird.Core.Services;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceCreateCommandHandler : ICommandHandler<MediaResourceCreateCommand, MediaResource>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public MediaResourceCreateCommandHandler(
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public MediaResource HandleReturn(MediaResourceCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            try
            {
                string mediaType = DetemineMediaType(command);

                var mediaResource = new MediaResource(
                                mediaType,
                                _documentSession.Load<User>(command.UserId),
                                command.UploadedOn);

                _documentSession.Store(mediaResource);

                switch (mediaType)
                {
                    case "image":
                        ImageDimensions imageDimensions;

                        var image = ImageUtility
                            .Load(command.Stream)
                            .GetImageDimensions(out imageDimensions);

                        MakeOriginalImageMediaResourceFile(mediaResource, command.OriginalFileName, command.Stream.Length, imageDimensions);

                        if (command.Usage == "observation")
                        {
                            MakeObservationImageMediaResourceFiles(mediaResource);
                        }
                        else if (command.Usage == "post")
                        {
                            MakePostImageMediaResourceFiles(mediaResource);
                        }
                        else if (command.Usage == "user")
                        {
                            MakeUserImageMediaResourceFiles(mediaResource);
                        }
                        else if (command.Usage == "group")
                        {
                            MakeGroupImageMediaResourceFiles(mediaResource);
                        }
                        else
                        {
                            MakeOtherImageMediaResourceFiles(mediaResource);
                        }

                        SaveImages(image, mediaResource);
                        break;
                }

                _documentSession.Store(mediaResource);

                return mediaResource;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string DetemineMediaType(MediaResourceCreateCommand command)
        {
            // TODO: Determine media type here, assume images only for now
            return "image";
        }

        private void MakeOriginalImageMediaResourceFile(MediaResource mediaResource, string originalFileName, long size, ImageDimensions imageDimensions)
        {
            string format = "jpeg"; // TODO: Handle formats other than JPEG
            string extension = "jpg";

            dynamic file = AddImageFile(mediaResource, "original", format, extension, imageDimensions.Width, imageDimensions.Height);
            file.Size = size.ToString();
            file.OriginalFilename = originalFileName;
        }

        private void MakeObservationImageMediaResourceFiles(MediaResource mediaResource)
        {
            AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);
        }

        private void MakePostImageMediaResourceFiles(MediaResource mediaResource)
        {
            AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);
        }

        private void MakeUserImageMediaResourceFiles(MediaResource mediaResource)
        {
            AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);
        }

        private void MakeGroupImageMediaResourceFiles(MediaResource mediaResource)
        {
            AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);
        }

        private void MakeOtherImageMediaResourceFiles(MediaResource mediaResource)
        {
            AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);
        }

        private MediaResourceFile AddImageFile(MediaResource mediaResource, string storedRepresentation, string format, string extension, int width, int height)
        {
            return mediaResource.AddImageFile(
                storedRepresentation,
                _mediaFilePathService.MakeMediaFileName(mediaResource.Id, storedRepresentation, extension),
                _mediaFilePathService.MakeRelativeMediaFileUri(mediaResource.Id, "image", storedRepresentation, extension),
                format,
                width,
                height,
                extension);
        }

        private void SaveImages(ImageUtility image, MediaResource mediaResource)
        {
            foreach (var file in mediaResource.Files)
            {
                dynamic imageFile = file.Value;

                var fullPath = _mediaFilePathService.MakeMediaFilePath(mediaResource.Id, "image", file.Key, imageFile.Extension);

                if (file.Key == "original")
                {
                    image
                        .SaveAs(fullPath);
                }
                else
                {
                    image
                        .Reset()
                        .Resize(new ImageDimensions(imageFile.Width, imageFile.Height), true, ImageResizeMode.Crop)
                        .SaveAs(fullPath);
                }
            }

            image.Cleanup();
        }

        #endregion
    }
}