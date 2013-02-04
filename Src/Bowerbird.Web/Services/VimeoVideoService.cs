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
using System.Net;
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
using Raven.Imports.Newtonsoft.Json;

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
        private readonly IDateTimeZoneService _dateTimeZoneService;

        private const string _uriFormat = @"http://player.vimeo.com/video/{0}?title=0&byline=0&portrait=0";
        private const string _apiUriFormat = @"http://vimeo.com/api/v2/video/{0}.json";

        #endregion

        #region Constructors

        public VimeoVideoService(
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

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, User createdByUser, out string failureReason, out MediaResource mediaResource)
        {
            failureReason = string.Empty;
            mediaResource = null;

            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).VimeoVideoServiceStatus)
            {
                failureReason = "Vimeo video files cannot be imported at the moment. Please try again later.";
                return false;
            }

            bool returnValue;

            try
            {
                string apiUri = string.Format(_apiUriFormat, command.VideoId);

                dynamic data = GetVideoDataFromApi(apiUri);

                // Get thumbnail URI
                var thumbnailUri = (string)data[0]["thumbnail_large"];
                var videoWidth = (int) data[0]["width"];
                var videoHeight = (int)data[0]["height"];

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
                        ImageDimensions.MakeRectangle(videoWidth, videoHeight),
                        thumbnailUri,
                        image.GetDimensions(),
                        MediaTypeUtility.GetStandardMimeTypeForMimeType(image.GetMimeType()),
                        GetVideoMetadata(data, command.VideoId),
                        imageCreationTasks);

                    image.Save(mediaResource, imageCreationTasks, _mediaFilePathFactory);

                    image.Cleanup();
                }

                returnValue = true;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving video", exception);

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
            DateTime convertedDateTime = _dateTimeZoneService.ExtractDateTimeFromExif((string)data[0]["upload_date"], "America/New_York");

            metadata.Add("Created", convertedDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            metadata.Add("Duration", ((int)data[0]["duration"]).ToString()); 
            metadata.Add("Description", (string)data[0]["title"]);

            return metadata;
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

                        return JsonConvert.DeserializeObject<dynamic>(data);
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