/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using System;
using Bowerbird.Core.Utilities;
using Raven.Client;

namespace Bowerbird.Core.Factories
{
    public class MediaResourceFactory : IMediaResourceFactory
    {
        #region Fields

        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MediaResourceFactory(
            IMediaFilePathFactory mediaFilePathFactory,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");
            Check.RequireNotNull(documentSession, "documentSession");

            _mediaFilePathFactory = mediaFilePathFactory;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public MediaResource MakeDefaultAvatarImage(AvatarDefaultType avatarType)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Image, null, DateTime.UtcNow, Guid.NewGuid().ToString(), new Dictionary<string, string>());
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            string avatarTypeName = avatarType.ToString().ToLower();
            string uriFormat = "/img/{0}-Square{1}.jpg";
 
            dynamic original = mediaResource.AddFile("Original", string.Format(uriFormat, avatarTypeName, 400), 400, 400);
            original.MimeType = Constants.ImageMimeTypes.Jpeg;

            mediaResource.AddFile("Square50", string.Format(uriFormat, avatarTypeName, 100), 100, 100);
            mediaResource.AddFile("Square100", string.Format(uriFormat, avatarTypeName, 200), 200, 200);
            mediaResource.AddFile("Square200", string.Format(uriFormat, avatarTypeName, 400), 400, 400);

            return mediaResource;
        }

        public MediaResource MakeDefaultBackgroundImage(string type)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Image, null, DateTime.UtcNow, Guid.NewGuid().ToString(), new Dictionary<string, string>());
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            string uriFormat = "/img/{0}-background-{1}.png";

            dynamic original = mediaResource.AddFile("Original", string.Format(uriFormat, type, "large"), 1024, 200);
            original.MimeType = Constants.ImageMimeTypes.Png;

            mediaResource.AddFile("Small", string.Format(uriFormat, type, "small"), 512, 100);
            mediaResource.AddFile("Large", string.Format(uriFormat, type, "large"), 1024, 200);

            return mediaResource;
        }

        public MediaResource MakeAvatarImage(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            ImageDimensions originalImageDimensions,
            string originalImageMimeType,
            List<ImageCreationTask> imageCreationTasks)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, new Dictionary<string, string>());
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            dynamic original = AddImageFile(mediaResource, "Original", originalImageMimeType, originalImageDimensions, null, imageCreationTasks);
            original.MimeType = originalImageMimeType;

            AddImageFile(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(100), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(200), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(400), ImageResizeMode.Crop, imageCreationTasks);

            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            return mediaResource;
        }

        public MediaResource MakeBackgroundImage(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            ImageDimensions originalImageDimensions,
            string originalImageMimeType,
            List<ImageCreationTask> imageCreationTasks)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, new Dictionary<string, string>());
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            dynamic original = AddImageFile(mediaResource, "Original", originalImageMimeType, originalImageDimensions, null, imageCreationTasks);
            original.MimeType = originalImageMimeType;

            AddImageFile(mediaResource, "Small", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeRectangle(512, 100), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Large", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeRectangle(1024, 200), ImageResizeMode.Crop, imageCreationTasks);

            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            return mediaResource;
        }

        public MediaResource MakeContributionImage(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            ImageDimensions originalImageDimensions,
            long originalSize,
            IDictionary<string, object> originalExifData,
            string originalImageMimeType,
            Dictionary<string, string> metadata,
            List<ImageCreationTask> imageCreationTasks)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            dynamic original = AddImageFile(mediaResource, "Original", originalImageMimeType, originalImageDimensions, null, imageCreationTasks);
            original.MimeType = originalImageMimeType;
            original.Filename = originalFileName;
            original.Size = originalSize;
            original.ExifData = originalExifData;

            AddImageFile(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(50), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(100), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(200), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Constrained240", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeRectangle(320, 240), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Constrained480", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeRectangle(640, 480), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Constrained600", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeRectangle(800, 600), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Full640", Constants.ImageMimeTypes.Jpeg, originalImageDimensions.ResizeWithTargetDimensions(640, 640), ImageResizeMode.Normal, imageCreationTasks);
            AddImageFile(mediaResource, "Full800", Constants.ImageMimeTypes.Jpeg, originalImageDimensions.ResizeWithTargetDimensions(800, 800), ImageResizeMode.Normal, imageCreationTasks);
            AddImageFile(mediaResource, "Full1024", Constants.ImageMimeTypes.Jpeg, originalImageDimensions.ResizeWithTargetDimensions(1024, 1024), ImageResizeMode.Normal, imageCreationTasks);

            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            return mediaResource;
        }

        public MediaResource MakeContributionExternalVideo(
            string key,
            User createdByUser,
            DateTime createdOn,
            string uri,
            string provider,
            object providerData,
            string videoId,
            ImageDimensions originalVideoDimensions,
            string originalThumbnailUri,
            ImageDimensions originalThumbnailDimensions,
            string originalThumbnailMimeType,
            Dictionary<string, string> metadata,
            List<ImageCreationTask> imageCreationTasks)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Video, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            dynamic originalImage = AddImageFile(mediaResource, "OriginalImage", originalThumbnailMimeType, originalThumbnailDimensions, null, imageCreationTasks);
            originalImage.MimeType = originalThumbnailMimeType;
            originalImage.Uri = originalThumbnailUri;

            AddImageFile(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(50), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(100), ImageResizeMode.Crop, imageCreationTasks);
            AddImageFile(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg, ImageDimensions.MakeSquare(200), ImageResizeMode.Crop, imageCreationTasks);

            dynamic original = mediaResource.AddFile("Original", uri, originalVideoDimensions.Width, originalVideoDimensions.Height);
            original.ProviderData = provider;
            original.VideoId = videoId;
            original.ProviderData = providerData;

            var constrained240Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(320, 240);
            var constrained480Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(640, 480);
            var constrained600Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(800, 600);
            var full640Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(640, 640);
            var full800Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(800, 800);
            var full1024Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(1024, 1024);

            mediaResource.AddFile("Constrained240", uri, constrained240Dimensions.Width, constrained240Dimensions.Height);
            mediaResource.AddFile("Constrained480", uri, constrained480Dimensions.Width, constrained480Dimensions.Height);
            mediaResource.AddFile("Constrained600", uri, constrained600Dimensions.Width, constrained600Dimensions.Height);
            mediaResource.AddFile("Full640", uri, full640Dimensions.Width, full640Dimensions.Height);
            mediaResource.AddFile("Full800", uri, full800Dimensions.Width, full800Dimensions.Height);
            mediaResource.AddFile("Full1024", uri, full1024Dimensions.Width, full1024Dimensions.Height);

            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            return mediaResource;
        }

        public MediaResource MakeContributionAudio(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            object id3Data,
            string standardMimeType,
            Dictionary<string, string> metadata)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Audio, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            var avatarUri = @"/img/audio-avatar.png";

            var audioFileUri = _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, "Original", MediaTypeUtility.GetStandardExtensionForMimeType(standardMimeType));

            dynamic original = mediaResource.AddFile("Original", audioFileUri, 400, 400);
            original.MimeType = Constants.ImageMimeTypes.Png;

            mediaResource.AddFile("Square50", avatarUri, 50, 50);
            mediaResource.AddFile("Square100", avatarUri, 100, 100);
            mediaResource.AddFile("Square200", avatarUri, 200, 200);
            mediaResource.AddFile("Constrained240", audioFileUri, 320, 240);
            mediaResource.AddFile("Constrained480", audioFileUri, 640, 480);
            mediaResource.AddFile("Constrained600", audioFileUri, 800, 600);
            mediaResource.AddFile("Full640", audioFileUri, 640, 480);
            mediaResource.AddFile("Full800", audioFileUri, 800, 600);
            mediaResource.AddFile("Full1024", audioFileUri, 1024, 768);

            return mediaResource;
        }

        public MediaResource MakeContributionDocument()
        {
            throw new NotImplementedException();
        }

        private MediaResourceFile AddImageFile(
            MediaResource mediaResource,
            string storedRepresentation,
            string mimeType,
            ImageDimensions imageDimensions,
            ImageResizeMode? imageResizeMode,
            List<ImageCreationTask> imageCreationTasks)
        {
            var file = mediaResource.AddFile(
                storedRepresentation,
                _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, storedRepresentation, MediaTypeUtility.GetStandardExtensionForMimeType(mimeType)),
                imageDimensions.Width,
                imageDimensions.Height);

            imageCreationTasks.Add(new ImageCreationTask
            {
                File = file,
                StoredRepresentation = storedRepresentation,
                DetermineBestOrientation = false, 
                ImageResizeMode = imageResizeMode,
                MimeType = mimeType
            });

            return file;
        }

        #endregion
    }
}