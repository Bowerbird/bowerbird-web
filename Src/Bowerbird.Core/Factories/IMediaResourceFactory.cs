using System;
using System.Collections.Generic;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Factories
{
}

namespace Bowerbird.Core.Factories
{
    public interface IMediaResourceFactory
    {
        MediaResource MakeDefaultAvatarImage(
            AvatarDefaultType avatarType);

        MediaResource MakeAvatarImage(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            int originalWidth,
            int originalHeight,
            string originalImageMimeType,
            List<ImageCreationTask> imageCreationTasks);

        MediaResource MakeContributionImage(
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
            List<ImageCreationTask> imageCreationTasks);

        MediaResource MakeContributionExternalVideo(
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
            List<ImageCreationTask> imageCreationTasks);

        MediaResource MakeContributionAudio();
        MediaResource MakeContributionDocument();
    }
}