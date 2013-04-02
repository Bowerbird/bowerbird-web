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

namespace Bowerbird.Core.DomainModelFactories
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
            var mediaResource = new ImageMediaResource(Constants.MediaResourceTypes.Image, null, DateTime.UtcNow, Guid.NewGuid().ToString(), null);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            string avatarTypeName = avatarType.ToString().ToLower();
            string uriFormat = "/img/{0}-Square{1}.jpg";
 
            mediaResource.Image.Original = MakeOriginalImageFile(Constants.ImageMimeTypes.Jpeg, string.Format(uriFormat, avatarTypeName, 400), 400, 400, null, null, null);
            mediaResource.Image.Square50 = MakeDerivedMediaResourceFile(string.Format(uriFormat, avatarTypeName, 100), 100, 100);
            mediaResource.Image.Square100 = MakeDerivedMediaResourceFile(string.Format(uriFormat, avatarTypeName, 200), 200, 200);
            mediaResource.Image.Square200 = MakeDerivedMediaResourceFile(string.Format(uriFormat, avatarTypeName, 400), 400, 400);

            return mediaResource;
        }

        public MediaResource MakeDefaultBackgroundImage(string type)
        {
            var mediaResource = new ImageMediaResource(Constants.MediaResourceTypes.Image, null, DateTime.UtcNow, Guid.NewGuid().ToString(), null);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            string uriFormat = "/img/{0}-background-{1}.png";

            mediaResource.Image.Original = MakeOriginalImageFile(Constants.ImageMimeTypes.Png, string.Format(uriFormat, type, "large"), 1024, 200, null, null, null);
            mediaResource.Image.Small = MakeDerivedMediaResourceFile(string.Format(uriFormat, type, "small"), 512, 100);
            mediaResource.Image.Large = MakeDerivedMediaResourceFile(string.Format(uriFormat, type, "large"), 1024, 200);

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
            var mediaResource = new ImageMediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, null);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            mediaResource.Image.Original = MakeOriginalImageFile(originalImageMimeType, MakeUri(mediaResource, "Original", originalImageMimeType), originalImageDimensions.Width, originalImageDimensions.Height, null, originalFileName, null);
            mediaResource.Image.Square50 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(100).Width, ImageDimensions.MakeSquare(100).Height, imageCreationTasks, "Square50", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Square100 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(200).Width, ImageDimensions.MakeSquare(200).Height, imageCreationTasks, "Square100", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Square200 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(400).Width, ImageDimensions.MakeSquare(400).Height, imageCreationTasks, "Square200", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);

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
            var mediaResource = new ImageMediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, null);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            mediaResource.Image.Original = MakeOriginalImageFile(originalImageMimeType, MakeUri(mediaResource, "Original", originalImageMimeType), originalImageDimensions.Width, originalImageDimensions.Height, null, originalFileName, null);
            mediaResource.Image.Small = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Small", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeRectangle(512, 100).Width, ImageDimensions.MakeRectangle(512, 100).Height, imageCreationTasks, "Small", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Large = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Large", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeRectangle(1024, 200).Width, ImageDimensions.MakeRectangle(1024, 200).Height, imageCreationTasks, "Large", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);

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
            var mediaResource = new ImageMediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            mediaResource.Image.Original = MakeOriginalImageFile(originalImageMimeType, MakeUri(mediaResource, "Original", originalImageMimeType), originalImageDimensions.Width, originalImageDimensions.Height, originalExifData, originalFileName, originalSize);
            mediaResource.Image.Square50 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(100).Width, ImageDimensions.MakeSquare(100).Height, imageCreationTasks, "Square50", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Square100 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(200).Width, ImageDimensions.MakeSquare(200).Height, imageCreationTasks, "Square100", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Square200 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(400).Width, ImageDimensions.MakeSquare(400).Height, imageCreationTasks, "Square200", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Constrained240 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Constrained240", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeRectangle(320, 240).Width, ImageDimensions.MakeRectangle(320, 240).Height, imageCreationTasks, "Constrained240", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Constrained480 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Constrained480", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeRectangle(640, 480).Width, ImageDimensions.MakeRectangle(640, 480).Height, imageCreationTasks, "Constrained480", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Constrained600 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Constrained600", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeRectangle(800, 600).Width, ImageDimensions.MakeRectangle(800, 600).Height, imageCreationTasks, "Constrained600", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Full640 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Full640", Constants.ImageMimeTypes.Jpeg), originalImageDimensions.ResizeWithTargetDimensions(640, 640).Width, originalImageDimensions.ResizeWithTargetDimensions(640, 640).Height, imageCreationTasks, "Full640", ImageResizeMode.Normal, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Full800 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Full800", Constants.ImageMimeTypes.Jpeg), originalImageDimensions.ResizeWithTargetDimensions(800, 800).Width, originalImageDimensions.ResizeWithTargetDimensions(800, 800).Height, imageCreationTasks, "Full800", ImageResizeMode.Normal, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Image.Full1024 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Full1024", Constants.ImageMimeTypes.Jpeg), originalImageDimensions.ResizeWithTargetDimensions(1024, 1024).Width, originalImageDimensions.ResizeWithTargetDimensions(1024, 1024).Height, imageCreationTasks, "Full1024", ImageResizeMode.Normal, Constants.ImageMimeTypes.Jpeg);

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
            var mediaResource = new VideoMediaResource(Constants.MediaResourceTypes.Video, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            var constrained240Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(320, 240);
            var constrained480Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(640, 480);
            var constrained600Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(800, 600);
            var full640Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(640, 640);
            var full800Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(800, 800);
            var full1024Dimensions = originalVideoDimensions.ResizeWithTargetDimensions(1024, 1024);

            mediaResource.Video.Original = MakeOriginalVideoFile(provider, videoId, providerData, originalVideoDimensions.Width, originalVideoDimensions.Height, uri);
            mediaResource.Video.OriginalImage = MakeOriginalImageFile(originalThumbnailMimeType, originalThumbnailUri, originalThumbnailDimensions.Width, originalThumbnailDimensions.Height, null, null, null);
            mediaResource.Video.Square50 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(100).Width, ImageDimensions.MakeSquare(100).Height, imageCreationTasks, "Square50", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Square100 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(200).Width, ImageDimensions.MakeSquare(200).Height, imageCreationTasks, "Square100", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Square200 = MakeDerivedMediaResourceFile(MakeUri(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg), ImageDimensions.MakeSquare(400).Width, ImageDimensions.MakeSquare(400).Height, imageCreationTasks, "Square200", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Constrained240 = MakeDerivedMediaResourceFile(uri, constrained240Dimensions.Width, constrained240Dimensions.Height, imageCreationTasks, "Constrained240", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Constrained480 = MakeDerivedMediaResourceFile(uri, constrained480Dimensions.Width, constrained480Dimensions.Height, imageCreationTasks, "Constrained480", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Constrained600 = MakeDerivedMediaResourceFile(uri, constrained600Dimensions.Width, constrained600Dimensions.Height, imageCreationTasks, "Constrained600", ImageResizeMode.Crop, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Full640 = MakeDerivedMediaResourceFile(uri, full640Dimensions.Width, full640Dimensions.Height, imageCreationTasks, "Full640", ImageResizeMode.Normal, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Full800 = MakeDerivedMediaResourceFile(uri, full800Dimensions.Width, full800Dimensions.Height, imageCreationTasks, "Full800", ImageResizeMode.Normal, Constants.ImageMimeTypes.Jpeg);
            mediaResource.Video.Full1024 = MakeDerivedMediaResourceFile(uri, full1024Dimensions.Width, full1024Dimensions.Height, imageCreationTasks, "Full1024", ImageResizeMode.Normal, Constants.ImageMimeTypes.Jpeg);

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
            var mediaResource = new AudioMediaResource(Constants.MediaResourceTypes.Audio, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            var avatarUri = @"/img/audio-avatar.png";
            var audioFileUri = _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, "Original", MediaTypeUtility.GetStandardExtensionForMimeType(standardMimeType));

            mediaResource.Audio.Original = MakeOriginalAudioFile(standardMimeType);
            mediaResource.Audio.Square50 = MakeDerivedMediaResourceFile(avatarUri, ImageDimensions.MakeSquare(100).Width, ImageDimensions.MakeSquare(100).Height);
            mediaResource.Audio.Square100 = MakeDerivedMediaResourceFile(avatarUri, ImageDimensions.MakeSquare(200).Width, ImageDimensions.MakeSquare(200).Height);
            mediaResource.Audio.Square200 = MakeDerivedMediaResourceFile(avatarUri, ImageDimensions.MakeSquare(400).Width, ImageDimensions.MakeSquare(400).Height);
            mediaResource.Audio.Constrained240 = MakeDerivedMediaResourceFile(audioFileUri, 100, 100);
            mediaResource.Audio.Constrained480 = MakeDerivedMediaResourceFile(audioFileUri, 200, 200);
            mediaResource.Audio.Constrained600 = MakeDerivedMediaResourceFile(audioFileUri, 400, 400);
            mediaResource.Audio.Full640 = MakeDerivedMediaResourceFile(audioFileUri, 640, 480);
            mediaResource.Audio.Full800 = MakeDerivedMediaResourceFile(audioFileUri, 800, 600);
            mediaResource.Audio.Full1024 = MakeDerivedMediaResourceFile(audioFileUri, 1024, 768);

            return mediaResource;
        }

        public MediaResource MakeContributionDocument()
        {
            throw new NotImplementedException();
        }

        private string MakeUri(MediaResource mediaResource, string storedRepresentation, string mimeType)
        {
            return _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, storedRepresentation, MediaTypeUtility.GetStandardExtensionForMimeType(mimeType));
        }

        public OriginalImageMediaResourceFile MakeOriginalImageFile(
            string mimeType,
            string uri,
            int width,
            int height,
            IDictionary<string, object> exifData,
            string filename,
            long? size)
        {
            return new OriginalImageMediaResourceFile()
            {
                MimeType = mimeType,
                Filename = filename,
                Width = width,
                Height = height,
                ExifData = exifData,
                Uri = uri,
                Size = size
            };
        }

        public OriginalVideoMediaResourceFile MakeOriginalVideoFile(
            string provider,
            string videoId,
            object providerData,
            int width,
            int height,
            string uri)
        {
            return new OriginalVideoMediaResourceFile()
                {
                    Provider = provider,
                    VideoId = videoId,
                    ProviderData = providerData,
                    Width = width,
                    Height = height,
                    Uri = uri
                };
        }

        public OriginalAudioMediaResourceFile MakeOriginalAudioFile(
            string mimeType)
        {
            return new OriginalAudioMediaResourceFile()
            {
                MimeType = mimeType
            };
        }

        public DerivedMediaResourceFile MakeDerivedMediaResourceFile(
            string uri,
            int width,
            int height,
            List<ImageCreationTask> imageCreationTasks = null,
            string storedRepresentation = null,
            ImageResizeMode? imageResizeMode = null,
            string mimeType = null)
        {
            var file = new DerivedMediaResourceFile()
            {
                Uri = uri,
                Height = height,
                Width = width
            };

            if (imageCreationTasks != null)
            {
                imageCreationTasks.Add(new ImageCreationTask
                    {
                        File = file,
                        StoredRepresentation = storedRepresentation,
                        DetermineBestOrientation = false,
                        ImageResizeMode = imageResizeMode,
                        MimeType = mimeType
                    });
            }

            return file;
        }

        #endregion
    }
}