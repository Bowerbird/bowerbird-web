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
using System.Collections.Generic;

namespace Bowerbird.Core.CommandHandlers
{
    public class MediaResourceCreateCommandHandler : ICommandHandler<MediaResourceCreateCommand, MediaResource>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;

        private class ImageCreationTask
        {
            public MediaResourceFile File { get; set; }
            public string StoredRepresentation { get; set; }
            public bool? DetermineBestOrientation { get; set; }
            public ImageResizeMode? ImageResizeMode { get; set; }

            public bool DoImageManipulation()
            {
                return DetermineBestOrientation.HasValue && ImageResizeMode.HasValue;
            }
        }

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

                        IDictionary<string, object> exifData;
                        var imageCreationTasks = new List<ImageCreationTask>();

                        var image = ImageUtility
                            .Load(command.Stream)
                            .GetImageDimensions(out imageDimensions)
                            .GetExifData(out exifData);

                        if(exifData != null && exifData.Count > 0)
                        {
                            mediaResource.AddExifData(exifData);
                        }

                        MakeOriginalImageMediaResourceFile(mediaResource, imageCreationTasks, command.OriginalFileName, command.Stream.Length, imageDimensions);

                        if (command.Usage == "observation")
                        {
                            MakeObservationImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else if (command.Usage == "post")
                        {
                            MakePostImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else if (command.Usage == "user")
                        {
                            MakeUserImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else if (command.Usage == "group")
                        {
                            MakeGroupImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }
                        else
                        {
                            MakeOtherImageMediaResourceFiles(mediaResource, imageCreationTasks);
                        }

                        SaveImages(image, mediaResource, imageCreationTasks);
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

        private void MakeOriginalImageMediaResourceFile(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks, string originalFileName, long size, ImageDimensions imageDimensions)
        {
            string format = "jpeg"; // TODO: Handle formats other than JPEG
            string extension = "jpg";

            dynamic file = AddImageFile(mediaResource, imageCreationTasks, "original", format, extension, imageDimensions.Width, imageDimensions.Height, null, null);
            file.Size = size.ToString();
            file.OriginalFilename = originalFileName;
        }

        private void MakeObservationImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            //AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            //AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            //AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            //AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);

            //AddImageFile(mediaResource, "ThumbnailSmall", "jpeg", "jpg", 42, 42);
            //AddImageFile(mediaResource, "ThumbnailMedium", "jpeg", "jpg", 100, 100);
            //AddImageFile(mediaResource, "ThumbnailLarge", "jpeg", "jpg", 200, 200);
            //AddImageFile(mediaResource, "FullSmall", "jpeg", "jpg", 130, 120);
            //AddImageFile(mediaResource, "FullMedium", "jpeg", "jpg", 130, 120);
            //AddImageFile(mediaResource, "FullLarge", "jpeg", "jpg", 130, 120);

            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailSmall", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailMedium", "jpeg", "jpg", 100, 100, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "ThumbnailLarge", "jpeg", "jpg", 200, 200, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "FullSmall", "jpeg", "jpg", 640, 480, false, ImageResizeMode.Normal);
            AddImageFile(mediaResource, imageCreationTasks, "FullMedium", "jpeg", "jpg", 1024, 768, false, ImageResizeMode.Normal);
            AddImageFile(mediaResource, imageCreationTasks, "FullLarge", "jpeg", "jpg", 1280, 1024, false, ImageResizeMode.Normal);
        }

        private void MakePostImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            //AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            //AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            //AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            //AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);

            AddImageFile(mediaResource, imageCreationTasks, "thumbnail", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "small", "jpeg", "jpg", 130, 120, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "medium", "jpeg", "jpg", 670, 600, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "large", "jpeg", "jpg", 1600, 1200, false, ImageResizeMode.Crop);
        }

        private void MakeUserImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            //AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            //AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            //AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            //AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);

            AddImageFile(mediaResource, imageCreationTasks, "thumbnail", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "small", "jpeg", "jpg", 130, 120, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "medium", "jpeg", "jpg", 670, 600, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "large", "jpeg", "jpg", 1600, 1200, false, ImageResizeMode.Crop);
        }

        private void MakeGroupImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            //AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            //AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            //AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            //AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);

            AddImageFile(mediaResource, imageCreationTasks, "thumbnail", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "small", "jpeg", "jpg", 130, 120, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "medium", "jpeg", "jpg", 670, 600, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "large", "jpeg", "jpg", 1600, 1200, false, ImageResizeMode.Crop);
        }

        private void MakeOtherImageMediaResourceFiles(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            //AddImageFile(mediaResource, "thumbnail", "jpeg", "jpg", 42, 42);
            //AddImageFile(mediaResource, "small", "jpeg", "jpg", 130, 120);
            //AddImageFile(mediaResource, "medium", "jpeg", "jpg", 670, 600);
            //AddImageFile(mediaResource, "large", "jpeg", "jpg", 1600, 1200);

            AddImageFile(mediaResource, imageCreationTasks, "thumbnail", "jpeg", "jpg", 42, 42, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "small", "jpeg", "jpg", 130, 120, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "medium", "jpeg", "jpg", 670, 600, false, ImageResizeMode.Crop);
            AddImageFile(mediaResource, imageCreationTasks, "large", "jpeg", "jpg", 1600, 1200, false, ImageResizeMode.Crop);
        }

        private MediaResourceFile AddImageFile(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks, string storedRepresentation, string format, string extension, int width, int height, bool? determineBestOrientation, ImageResizeMode? imageResizeMode)
        {
            var mediaResourceFile = mediaResource.AddImageFile(
                storedRepresentation,
                _mediaFilePathService.MakeMediaFileName(mediaResource.Id, storedRepresentation, extension),
                _mediaFilePathService.MakeRelativeMediaFileUri(mediaResource.Id, "image", storedRepresentation, extension),
                format,
                width,
                height,
                extension);

            imageCreationTasks.Add(new ImageCreationTask 
                { 
                    File = mediaResourceFile, 
                    StoredRepresentation = storedRepresentation,
                    DetermineBestOrientation = determineBestOrientation, 
                    ImageResizeMode = imageResizeMode
                });

            return mediaResourceFile;
        }

        private void SaveImages(ImageUtility image, MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks)
        {
            foreach (var imageCreationTask in imageCreationTasks)
            {
                dynamic imageFile = imageCreationTask.File;

                var fullPath = _mediaFilePathService.MakeMediaFilePath(mediaResource.Id, "image", imageCreationTask.StoredRepresentation, imageFile.Extension);

                if (!imageCreationTask.DoImageManipulation())
                {
                    image
                        .Reset()
                        .SaveAs(fullPath);
                }
                else
                {
                    image
                        .Reset()
                        .Resize(new ImageDimensions(imageFile.Width, imageFile.Height), imageCreationTask.DetermineBestOrientation.Value, imageCreationTask.ImageResizeMode.Value)
                        .SaveAs(fullPath);
                }
            }

            image.Cleanup();
        }

        #endregion
    }
}