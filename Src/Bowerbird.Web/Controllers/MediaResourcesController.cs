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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.Config;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Controllers
{
    public class MediaResourcesController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MediaResourcesController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult ObservationUpload(string key, string originalFileName, HttpPostedFileBase file)
        {
            return ProcessPostedImage(key, originalFileName, file, "observation");
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostUpload(string key, string originalFileName, HttpPostedFileBase file)
        {
            return ProcessPostedImage(key, originalFileName, file, "post");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AvatarUpload(HttpPostedFileBase file)
        {
            return ProcessPostedImage(string.Empty, string.Empty, file, "avatar");
        }

        private ActionResult ProcessPostedImage(string key, string originalFileName, HttpPostedFileBase file, string recordType)
        {
            try
            {
                var mediaResourceCreateCommand = new MediaResourceCreateCommand()
                {
                    OriginalFileName = originalFileName ?? string.Empty,
                    Stream = file.InputStream,
                    UploadedOn = DateTime.UtcNow,
                    Usage = recordType,
                    UserId = _userContext.GetAuthenticatedUserId()
                };

                MediaResource mediaResource = null;

                _commandProcessor.Process<MediaResourceCreateCommand, MediaResource>(mediaResourceCreateCommand, x => { mediaResource = x; });

                _documentSession.SaveChanges();

                string photoDateTime, photoLatitude, photoLongitude;

                SetExifData(mediaResource, out photoDateTime, out photoLatitude, out photoLongitude);

                return new JsonNetResult(new
                    {
                        mediaResource.Id,
                        mediaResource.CreatedByUser,
                        mediaResource.Type,
                        mediaResource.UploadedOn,
                        mediaResource.Files,
                        PhotoDateTime = photoDateTime,
                        PhotoLatitude = photoLatitude,
                        PhotoLongitude = photoLongitude,
                        Key = key
                    });
            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { success = false, error = ex.Message });
            }
        }

        private void SetExifData(MediaResource mediaResource, out string dateTime, out string lat, out string lon)
        {
            lat = string.Empty;
            lon = string.Empty;

            if(mediaResource.Exifdata.ContainsKey(ExifLib.ExifTags.GPSLatitude.ToString()) &&
                mediaResource.Exifdata.ContainsKey(ExifLib.ExifTags.GPSLongitude.ToString()) &&
                mediaResource.Exifdata.ContainsKey(ExifLib.ExifTags.GPSLatitudeRef.ToString()) &&
                mediaResource.Exifdata.ContainsKey(ExifLib.ExifTags.GPSLongitudeRef.ToString()))
            {
                var latitudeConverted = ConvertDegreeAngleToDouble(
                    mediaResource.Exifdata[ExifLib.ExifTags.GPSLatitude.ToString()] as double[],
                    mediaResource.Exifdata[ExifLib.ExifTags.GPSLatitudeRef.ToString()] as string);
                
                var longitudeConverted = ConvertDegreeAngleToDouble(
                    mediaResource.Exifdata[ExifLib.ExifTags.GPSLongitude.ToString()] as double[],
                    mediaResource.Exifdata[ExifLib.ExifTags.GPSLongitudeRef.ToString()] as string);

                if (latitudeConverted.HasValue && longitudeConverted.HasValue)
                {
                    lat = latitudeConverted.Value.ToString();
                    lon = longitudeConverted.Value.ToString();
                }
            }

            if (mediaResource.Exifdata.ContainsKey(ExifLib.ExifTags.DateTime.ToString()))
            {
                var convertedDateTime = ConvertDateTime(mediaResource.Exifdata[ExifLib.ExifTags.DateTime.ToString()].ToString());

                dateTime = convertedDateTime.ToString("dd MMM yyyy");
            }
            else
            {
                dateTime = DateTime.UtcNow.ToString("dd MMM yyyy");
            }
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

        // DateTime is stored as a string - "yyyy:MM:dd hh:mm:ss" in 24 hour format
        private DateTime ConvertDateTime(string dateTimeExif)
        {
            var dateTimeStringComponents = dateTimeExif.Split(new[] { ':', ' ' });

            if (dateTimeExif != string.Empty && dateTimeStringComponents.Count() == 6)
            {
                var dateTimeIntComponents = new int[dateTimeStringComponents.Count()];

                for (var i = 0; i < dateTimeStringComponents.Length; i++)
                {
                    int convertedSegment;
                    if (Int32.TryParse(dateTimeStringComponents[i], out convertedSegment))
                    {
                        dateTimeIntComponents[i] = convertedSegment;
                    }
                }

                return new DateTime(
                    dateTimeIntComponents[0], // year
                    dateTimeIntComponents[1], // month
                    dateTimeIntComponents[2], // day
                    dateTimeIntComponents[3], // hour
                    dateTimeIntComponents[4], // minute
                    dateTimeIntComponents[5] // second
                    );
            }

            return DateTime.UtcNow;
        }

        #endregion
    }
}