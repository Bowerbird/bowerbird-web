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
using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModelFactories;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Services;
using Bowerbird.Core.Utilities;
using NLog;
using Raven.Client;

namespace Bowerbird.Web.Services
{
    public class ImageService : IImageService
    {
        #region Fields

        private Logger _logger = LogManager.GetLogger("ImageService");

        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IDocumentSession _documentSession;
        private readonly IDateTimeZoneService _dateTimeZoneService;

        #endregion

        #region Constructors

        public ImageService(
            IMediaFilePathFactory mediaFilePathFactory,
            IMediaResourceFactory mediaResourceFactory,
            IDocumentSession documentSession,
            IDateTimeZoneService dateTimeZoneService
            )
        {
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(dateTimeZoneService, "dateTimeZoneService");

            _mediaFilePathFactory = mediaFilePathFactory;
            _mediaResourceFactory = mediaResourceFactory;
            _documentSession = documentSession;
            _dateTimeZoneService = dateTimeZoneService;
        }

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, User createdByUser, out string failureReason, out MediaResource mediaResource)
        {
            failureReason = string.Empty;
            mediaResource = null;

            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).ImageServiceStatus)
            {
                failureReason = "Image files cannot be uploaded at the moment. Please try again later.";
                return false;
            }
            
            ImageUtility image = null;
            bool returnValue;

            try
            {
                var imageCreationTasks = new List<ImageCreationTask>();

                image = ImageUtility.Load(command.FileStream);

                if (command.Usage == "contribution")
                {
                    mediaResource = MakeContributionImage(
                        createdByUser,
                        image,
                        imageCreationTasks,
                        command);
                }
                else if (command.Usage == "avatar")
                {
                    mediaResource = MakeAvatarImage(
                        createdByUser,
                        image,
                        imageCreationTasks,
                        command);
                }
                else if (command.Usage == "background")
                {
                    mediaResource = MakeBackgroundImage(
                        createdByUser,
                        image,
                        imageCreationTasks,
                        command);
                }
                else
                {
                    throw new ArgumentException("The specified usage '" + command.Usage + "' is not recognised.");
                }

                image.Save(mediaResource, imageCreationTasks, _mediaFilePathFactory);

                returnValue = true;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving images", exception);

                failureReason = "The file is corrupted or not a valid JPEG and could not be saved. Please check the file and try again.";
                returnValue = false;
            }
            finally
            {
                if (image != null)
                {
                    image.Cleanup();
                }
            }

            return returnValue;
        }

        private MediaResource MakeContributionImage(User createdByUser, ImageUtility image, List<ImageCreationTask> imageCreationTasks, MediaResourceCreateCommand command)
        {
            IDictionary<string, object> exifData = image.GetExifData();
            ImageDimensions imageDimensions = image.GetDimensions();

            var metadata = new Dictionary<string, string>();

            if (exifData.Count > 0)
            {
                metadata = GetImageExifMetadata(exifData, createdByUser.Timezone);
            }

            return _mediaResourceFactory.MakeContributionImage(
                command.Key,
                createdByUser,
                command.UploadedOn,
                command.FileName,
                imageDimensions,
                command.FileStream.Length,
                exifData,
                image.GetMimeType(),
                metadata,
                imageCreationTasks);
        }

        private MediaResource MakeAvatarImage(User createdByUser, ImageUtility image, List<ImageCreationTask> imageCreationTasks, MediaResourceCreateCommand command)
        {
            ImageDimensions imageDimensions = image.GetDimensions();

            return _mediaResourceFactory.MakeAvatarImage(
                command.Key,
                createdByUser,
                command.UploadedOn,
                command.FileName,
                imageDimensions,
                image.GetMimeType(),
                imageCreationTasks);
        }

        private MediaResource MakeBackgroundImage(User createdByUser, ImageUtility image, List<ImageCreationTask> imageCreationTasks, MediaResourceCreateCommand command)
        {
            ImageDimensions imageDimensions = image.GetDimensions();

            return _mediaResourceFactory.MakeBackgroundImage(
                command.Key,
                createdByUser,
                command.UploadedOn,
                command.FileName,
                imageDimensions,
                image.GetMimeType(),
                imageCreationTasks);
        }

        private Dictionary<string, string> GetImageExifMetadata(IDictionary<string, object> exifData, string timezone)
        {
            var metadata = new Dictionary<string, string>();

            if (exifData.ContainsKey(ExifTags.GPSLatitude.ToString()) &&
                exifData.ContainsKey(ExifTags.GPSLongitude.ToString()) &&
                exifData.ContainsKey(ExifTags.GPSLatitudeRef.ToString()) &&
                exifData.ContainsKey(ExifTags.GPSLongitudeRef.ToString()))
            {
                var latitudeConverted = ConvertDegreeAngleToDouble(
                    exifData[ExifTags.GPSLatitude.ToString()] as double[],
                    exifData[ExifTags.GPSLatitudeRef.ToString()] as string);

                var longitudeConverted = ConvertDegreeAngleToDouble(
                    exifData[ExifTags.GPSLongitude.ToString()] as double[],
                    exifData[ExifTags.GPSLongitudeRef.ToString()] as string);

                if (latitudeConverted.HasValue && longitudeConverted.HasValue)
                {
                    metadata.Add("Latitude", latitudeConverted.Value.ToString());
                    metadata.Add("Longitude", longitudeConverted.Value.ToString());
                }
            }

            if (exifData.ContainsKey(ExifTags.DateTimeOriginal.ToString()))
            {
                DateTime convertedDateTime = _dateTimeZoneService.ExtractDateTimeFromExif(exifData[ExifTags.DateTimeOriginal.ToString()].ToString(), timezone);

                metadata.Add("Created", convertedDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            }

            return metadata;
        }

        // In geographic coordinates stored as EXIF data, the coordinates for latitude and longitude are stored
        // as unsigned doubles in an array. Latitudes are the meridian lines parallel with the equator. A negative latitude
        // will hence fall under the equator. Longitudes are perpendicular to the equator. A negative longitude will occur between
        // Grenich in the UK (GMD) and the international Date Line which is in the middle of the Pacific Ocean.
        // Australian latitudes are thus negative and longitudes are positive. In the EXIF data, a latitude is stored in a latitude array,
        // and it's location relative to grenich is stored in a separate field - in our case "GPSLatitudeRef". A ref of "N" will indicate +'ive.
        // A ref of "S" will indicate that the Latitude should be -'ive. The Longitude will have an "GPSLongitudeRef" of "E" for +'ive, "W" for -'ive.
        private double? ConvertDegreeAngleToDouble(double[] coordinates, string orientation)
        {
            if (coordinates == null) return null;

            double degrees = coordinates[0], minutes = coordinates[1], seconds = coordinates[2];

            var negative = orientation.ToLower().Equals("w") || orientation.ToLower().Equals("s");

            if (negative)
            {
                // turn it positive, apply arithmetic, then return as negative again
                return (-1 * degrees) - (minutes / 60) - (seconds / 3600);
            }

            return degrees + (minutes / 60) + (seconds / 3600);
        }

        #endregion
    }
}