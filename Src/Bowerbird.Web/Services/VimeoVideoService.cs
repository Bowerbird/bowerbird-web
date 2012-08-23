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
using System.IO;
using System.Linq;
using System.Net;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;
using Bowerbird.Core.Factories;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Services;
using Bowerbird.Web.Utilities;
using NLog;
using Newtonsoft.Json.Linq;
using NodaTime;
using Raven.Client;

namespace Bowerbird.Web.Services
{
    /// <summary>
    /// This service contains logic specific to querying the Vimeo API to return a video and it's data
    /// Examples of its use can be found at: 
    /// http://developer.vimeo.com/apis/simple#activity-response-example
    /// </summary>
    public class VimeoVideoService : IVimeoVideoService
    {

        #region Members

        private Logger _logger = LogManager.GetLogger("VimeoVideoService");

        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IDocumentSession _documentSession;
        private readonly IMessageBus _messageBus;

        private const string _uriFormat = @"http://player.vimeo.com/video/{0}?title=0&byline=0&portrait=0";
        private const string _apiUriFormat = @"http://vimeo.com/api/v2/video/{0}.json";

        #endregion

        #region Constructors

        public VimeoVideoService(
            IMediaFilePathFactory mediaFilePathFactory,
            IMediaResourceFactory mediaResourceFactory,
            IDocumentSession documentSession,
            IMessageBus messageBus
            )
        {
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(messageBus, "messageBus");

            _mediaFilePathFactory = mediaFilePathFactory;
            _mediaResourceFactory = mediaResourceFactory;
            _documentSession = documentSession;
            _messageBus = messageBus;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, out string failureReason)
        {
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).VimeoVideoServiceStatus)
            {
                failureReason = "Vimeo video files cannot be imported at the moment. Please try again later.";
                return false;
            }

            MediaResource mediaResource = null;
            bool returnValue;

            try
            {
                string apiUri = string.Format(_apiUriFormat, command.VideoId);

                dynamic data = GetVideoDataFromApi(apiUri);

                // Get thumbnail URI
                var thumbnailUri = (string)data[0]["thumbnail_large"];
                var videoWidth = (int) data[0]["width"];
                var videoHeight = (int)data[0]["height"];

                var createdByUser = _documentSession.Load<User>(command.UserId);

                var imageCreationTasks = new List<ImageCreationTask>();

                using (var stream = new MemoryStream(new WebClient().DownloadData(thumbnailUri)))
                {
                    var image = ImageUtility.Load(stream);

                    mediaResource = _mediaResourceFactory.MakeContributionExternalVideo(
                        command.Key,
                        createdByUser,
                        command.UploadedOn,
                        string.Format(_uriFormat, command.VideoId),
                        "vimeo",
                        data,
                        command.VideoId,
                        videoWidth,
                        videoHeight,
                        thumbnailUri,
                        image.GetImageDimensions().Width,
                        image.GetImageDimensions().Height,
                        MediaTypeUtility.GetStandardMimeTypeForFile(stream),
                        GetVideoMetadata(data, command.VideoId),
                        imageCreationTasks);

                    FileUtility.SaveImages(image, mediaResource, imageCreationTasks, _mediaFilePathFactory);

                    image.Cleanup();
                }

                _documentSession.Store(mediaResource);
                _documentSession.SaveChanges();

                _messageBus.Publish(new DomainModelCreatedEvent<MediaResource>(mediaResource, createdByUser, mediaResource));

                failureReason = string.Empty;
                returnValue = true;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving video", exception);

                if (mediaResource != null)
                {
                    _documentSession.Delete(mediaResource);
                    _documentSession.SaveChanges();
                }
                
                failureReason = "The video cannot be retrieved from Vimeo. Please check the video and try again.";
                returnValue = false;
            }

            return returnValue;
        }

        private Dictionary<string, string> GetVideoMetadata(dynamic data, string videoId)
        {
            var metadata = new Dictionary<string, string>();

            metadata.Add("Provider", "vimeo");
            metadata.Add("VideoId", videoId);

            // Vimeo's dodgy API doesn't include timezone info for upload date. 
            // According to this random link (http://vimeo.com/forums/topic:47127), some "staff" member 
            // says "Eastern", which probably means US eastern time ("America/New_York" in the tzdb that NodaTime uses). :(
            DateTime convertedDateTime = ConvertDateTime((string)data[0]["upload_date"], "America/New_York");

            metadata.Add("Created", convertedDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            metadata.Add("Duration", ((int)data[0]["duration"]).ToString()); 
            metadata.Add("Description", (string)data[0]["title"]);

            return metadata;
        }

        /// <summary>
        /// EXIF DateTime is stored as a string - "yyyy-MM-dd hh:mm:ss" in 24 hour format.
        /// 
        /// Since EXIF datetime does not store timezone info, making the time ambiguous, we grab the user's specified timezone
        /// and assume the image was taken in that timezone.
        /// 
        /// If we don't have a parseable datetime, we set the time to now.
        /// </summary>
        private DateTime ConvertDateTime(string dateTimeExif, string timezone)
        {
            var dateTimeStringComponents = dateTimeExif.Split(new[] { ':', ' ', '-' });

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

                // Get data into a local time object (no time zone specified)
                var localDateTime = new LocalDateTime(
                        dateTimeIntComponents[0], // year
                        dateTimeIntComponents[1], // month
                        dateTimeIntComponents[2], // day
                        dateTimeIntComponents[3], // hour
                        dateTimeIntComponents[4], // minute
                        dateTimeIntComponents[5], // second
                        CalendarSystem.Iso);

                // Put the local date time into a timezone
                var zonedDateTime = localDateTime.InZoneLeniently(DateTimeZoneProviders.Tzdb[timezone]);

                // Get the UTC date time of the given local date time in the given time zone
                return zonedDateTime.WithZone(DateTimeZone.Utc).ToDateTimeUtc();
            }

            return DateTime.UtcNow;
        }

        /// <summary>
        /// Using a web client, grab the video data from the service api try and pull the data 3 times before failing.
        /// </summary>
        protected dynamic GetVideoDataFromApi(string apiCall)
        {
            const int apiRequestAttempts = 3;

            using (var apiWebClient = new WebClient())
            {
                int apiRequestCount = 1;

                while (apiRequestCount < apiRequestAttempts)
                {
                    try
                    {
                        var data = apiWebClient.DownloadString(apiCall);

                        return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(data);
                    }
                    catch (Exception exception)
                    {
                        _logger.ErrorException("Error requesting video", exception);

                        apiRequestCount++;
                    }
                }
            }

            return null;
        }

        #endregion

    }
}