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

            var fileName = string.Format("default-{0}-avatar.jpg", avatarType.ToString().ToLower());
            var uri = string.Format("/img/{0}", fileName);
             
            dynamic original = mediaResource.AddFile("Original", uri, 400, 400);
            original.MimeType = Constants.ImageMimeTypes.Jpeg;

            mediaResource.AddFile("Square50", uri, 50, 50);
            mediaResource.AddFile("Square100", uri, 100, 100);
            mediaResource.AddFile("Square200", uri, 200, 200);

            return mediaResource;
        }

        public MediaResource MakeAvatarImage(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            int originalWidth,
            int originalHeight,
            string originalImageMimeType,
            List<ImageCreationTask> imageCreationTasks)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, new Dictionary<string, string>());
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            dynamic original = AddImageFile(mediaResource, "Original", originalImageMimeType, originalWidth, originalHeight, null, null, imageCreationTasks);
            original.MimeType = originalImageMimeType;

            AddImageFile(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg, 50, 50, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg, 100, 100, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg, 200, 200, false, "crop", imageCreationTasks);

            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            return mediaResource;
        }

        public MediaResource MakeContributionImage(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            int originalWidth,
            int originalHeight,
            long originalSize,
            IDictionary<string, object> originalExifData,
            string originalImageMimeType,
            Dictionary<string, string> metadata,
            List<ImageCreationTask> imageCreationTasks)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Image, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            dynamic original = AddImageFile(mediaResource, "Original", originalImageMimeType, originalWidth, originalHeight, null, null, imageCreationTasks);
            original.MimeType = originalImageMimeType;
            original.Filename = originalFileName;
            original.Size = originalSize;
            original.ExifData = originalExifData;

            AddImageFile(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg, 50, 50, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg, 100, 100, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg, 200, 200, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Constrained240", Constants.ImageMimeTypes.Jpeg, 320, 240, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Constrained480", Constants.ImageMimeTypes.Jpeg, 640, 480, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Constrained600", Constants.ImageMimeTypes.Jpeg, 800, 600, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Full640", Constants.ImageMimeTypes.Jpeg, 640, 640, true, "normal", imageCreationTasks);
            AddImageFile(mediaResource, "Full800", Constants.ImageMimeTypes.Jpeg, 800, 800, true, "normal", imageCreationTasks);
            AddImageFile(mediaResource, "Full1024", Constants.ImageMimeTypes.Jpeg, 1024, 1024, true, "normal", imageCreationTasks);

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
            int videoWidth,
            int videoHeight,
            string originalThumbnailUri,
            int originalThumbnailWidth,
            int originalThumbnailHeight,
            string originalThumbnailMimeType,
            Dictionary<string, string> metadata,
            List<ImageCreationTask> imageCreationTasks)
        {
            var mediaResource = new MediaResource(Constants.MediaResourceTypes.Video, createdByUser, createdOn, key, metadata);
            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            dynamic originalImage = AddImageFile(mediaResource, "OriginalImage", originalThumbnailMimeType, originalThumbnailWidth, originalThumbnailHeight, null, null, imageCreationTasks);
            originalImage.MimeType = originalThumbnailMimeType;
            originalImage.Uri = originalThumbnailUri;

            AddImageFile(mediaResource, "Square50", Constants.ImageMimeTypes.Jpeg, 50, 50, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Square100", Constants.ImageMimeTypes.Jpeg, 100, 100, false, "crop", imageCreationTasks);
            AddImageFile(mediaResource, "Square200", Constants.ImageMimeTypes.Jpeg, 200, 200, false, "crop", imageCreationTasks);

            dynamic original = mediaResource.AddFile("Original", uri, videoWidth, videoHeight);
            original.ProviderData = provider;
            original.VideoId = videoId;
            original.ProviderData = providerData;

            mediaResource.AddFile("Constrained240", uri, 320, 240);
            mediaResource.AddFile("Constrained480", uri, 640, 480);
            mediaResource.AddFile("Constrained600", uri, 800, 600);
            mediaResource.AddFile("Full640", uri, 640, 640);
            mediaResource.AddFile("Full800", uri, 800, 800);
            mediaResource.AddFile("Full1024", uri, 1024, 1024);

            _documentSession.Store(mediaResource);
            _documentSession.SaveChanges();

            return mediaResource;
        }

        public MediaResource MakeContributionAudio()
        {
            throw new NotImplementedException();
        }

        public MediaResource MakeContributionDocument()
        {
            throw new NotImplementedException();
        }

        private MediaResourceFile AddImageFile(
            MediaResource mediaResource,
            string storedRepresentation,
            string mimeType,
            int width,
            int height,
            bool? determineBestOrientation,
            string imageResizeMode,
            List<ImageCreationTask> imageCreationTasks)
        {
            // original height / original width x new width = new height
            // original width / original height x new height = new width

            //var newHeight = (height / width) * 640;
            //var newWidth = (width / height) * 640;

            var file = mediaResource.AddFile(
                storedRepresentation,
                _mediaFilePathFactory.MakeRelativeMediaFileUri(mediaResource.Id, storedRepresentation, MediaTypeUtility.GetStandardExtensionForMimeType(mimeType)),
                width,
                height);

            imageCreationTasks.Add(new ImageCreationTask
            {
                File = file,
                StoredRepresentation = storedRepresentation,
                DetermineBestOrientation = determineBestOrientation,
                ImageResizeMode = imageResizeMode,
                MimeType = mimeType
            });

            return file;
        }

        #endregion
    }
}