using System;
using System.Collections.Generic;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Utilities;

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
            ImageDimensions originalImageDimensions,
            string originalImageMimeType,
            List<ImageCreationTask> imageCreationTasks);

        MediaResource MakeContributionImage(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            ImageDimensions originalImageDimensions,
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
            ImageDimensions originalVideoDimensions,
            string originalThumbnailUri,
            ImageDimensions originalThumbnailDimensions,
            string originalThumbnailMimeType,
            Dictionary<string, string> metadata,
            List<ImageCreationTask> imageCreationTasks);

        MediaResource MakeContributionAudio(
            string key,
            User createdByUser,
            DateTime createdOn,
            string originalFileName,
            object id3Data,
            string standardMimeType,
            Dictionary<string, string> metadata);

        MediaResource MakeContributionDocument();
    }
}