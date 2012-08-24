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
using Bowerbird.Core.Utilities;
using NLog;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;

namespace Bowerbird.Web.Services
{
    /// <summary>
    /// This service contains logic specific to querying the Youtube API to return videos and metadata
    /// Examples of its use can be found at: 
    /// https://developers.google.com/youtube/v3/getting-started
    /// </summary>
    public class YouTubeVideoService : IYouTubeVideoService
    {

        #region Members

        private Logger _logger = LogManager.GetLogger("YouTubeVideoService");

        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IDocumentSession _documentSession;
        private readonly IMessageBus _messageBus;

        private const string _uriFormat = @"http://www.youtube.com/embed/{0}?controls=0&modestbranding=1&rel=0&showinfo=0";
        private const string _apiUriFormat = @"http://gdata.youtube.com/feeds/api/videos/{0}?v=2&alt=json";

        #endregion

        #region Constructors

        public YouTubeVideoService(
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
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).YouTubeVideoServiceStatus)
            {
                failureReason = "Youtube video files cannot be imported at the moment. Please try again later.";
                return false;
            }

            MediaResource mediaResource = null;
            bool returnValue;

            try
            {
                string apiUri = string.Format(_apiUriFormat, command.VideoId);

                JObject data = GetVideoDataFromApi(apiUri);

                // Get thumbnail URI
                var mediaThumbnails = data["entry"]["media$group"]["media$thumbnail"];
                var mediaThumbnail = mediaThumbnails.Single(x => (string)x["yt$name"] == "hqdefault");
                var thumbnailUri = (string)mediaThumbnail["url"];

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
                        "youtube",
                        data,
                        command.VideoId,
                        ImageDimensions.MakeRectangle(1024, 576), // As at 08/2012, Youtube states that videos are encoded in 16:9 ratio. 1024x576px is the max size we present in Bowerbird at that ratio
                        thumbnailUri,
                        image.GetDimensions(),
                        MediaTypeUtility.GetStandardMimeTypeForMimeType(image.GetMimeType()),
                        GetVideoMetadata(data, command.VideoId),
                        imageCreationTasks);

                    image.Save(mediaResource, imageCreationTasks, _mediaFilePathFactory);

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

                failureReason = "The video cannot be retrieved from Youtube. Please check the video and try again.";
                returnValue = false;
            }

            return returnValue;
        }

        private Dictionary<string, string> GetVideoMetadata(JObject data, string videoId)
        {
            var metadata = new Dictionary<string, string>();

            metadata.Add("Provider", "youtube");
            metadata.Add("VideoId", videoId);
            metadata.Add("Description", (string)data["entry"]["media$group"]["media$description"]["$t"]);
            metadata.Add("Duration", (string)data["entry"]["media$group"]["yt$duration"]["seconds"]);
            metadata.Add("Created", ((DateTime)data["entry"]["media$group"]["yt$uploaded"]["$t"]).ToString(Constants.ISO8601DateTimeFormat));

            return metadata;
        }

        /// <summary>
        /// Using a web client, grab the video data from the service api try and pull the data 3 times before failing.
        /// </summary>
        protected JObject GetVideoDataFromApi(string apiCall)
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