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
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using NLog;
using Raven.Client;

namespace Bowerbird.Web.Services
{
    /// <summary>
    /// This service contains logic specific to querying the Youtube API to return videos and metadata
    /// Examples of its use can be found at: 
    /// https://developers.google.com/youtube/v3/getting-started
    /// </summary>
    public class YouTubeVideoService : VideoServiceBase, IYouTubeVideoService
    {

        #region Members

        private Logger _logger = LogManager.GetLogger("YouTubeVideoService");

        private readonly IDocumentSession _documentSession;

        private const string _uriFormat = @"http://www.youtube.com/embed/{0}?controls=0&modestbranding=1&rel=0&showinfo=0";
        private const string _apiUriFormat = @"http://gdata.youtube.com/feeds/api/videos/{0}?v=2&alt=json";

        #endregion

        #region Constructors

        public YouTubeVideoService(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool Save(MediaResourceCreateCommand command, MediaResource mediaResource, out string failureReason)
        {
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).YouTubeVideoServiceStatus)
            {
                failureReason = "Vimeo video files cannot be imported at the moment. Please try again later.";
                return false;
            }

            try
            {
                string apiUri = string.Format(_apiUriFormat, command.VideoId);

                dynamic data = GetVideoDataFromApi(apiUri);

                //               {
                //                   {"title","Title"},
                //                   {"description","Description"},
                //                   {"duration","Duration"},
                //                   {"aspectRatio","Aspect"}
                //               };

                AddMetadata(mediaResource, "youtube", command.VideoId);

                MakeVideoMediaResourceFiles(mediaResource, data, string.Format(_uriFormat, command.VideoId), "youtube", command.VideoId);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving video", exception);

                failureReason = "The video cannot be retrieved from Vimeo. Please check the video and try again.";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        #endregion

    }
}