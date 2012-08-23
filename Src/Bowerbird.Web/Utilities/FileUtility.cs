using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Factories;

namespace Bowerbird.Web.Utilities
{
    public class FileUtility
    {

        public static void SaveImages(ImageUtility image, MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks, IMediaFilePathFactory mediaFilePathFactory)
        {
            foreach (var imageCreationTask in imageCreationTasks)
            {
                dynamic imageFile = imageCreationTask.File;

                var basePath = mediaFilePathFactory.MakeMediaBasePath(mediaResource.Id);

                if(!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                var fullPath = mediaFilePathFactory.MakeMediaFilePath(mediaResource.Id, imageCreationTask.StoredRepresentation, MediaTypeUtility.GetStandardExtensionForMimeType(imageCreationTask.MimeType));

                if (!imageCreationTask.DoImageManipulation())
                {
                    image
                        .Reset()
                        .SaveAs(imageCreationTask.MimeType, fullPath);
                }
                else
                {
                    image
                        .Reset()
                        .Resize(new ImageDimensions(imageFile.Width, imageFile.Height), imageCreationTask.DetermineBestOrientation.Value, imageCreationTask.ImageResizeMode)
                        .SaveAs(imageCreationTask.MimeType, fullPath);
                }
            }
        }

    }
}
