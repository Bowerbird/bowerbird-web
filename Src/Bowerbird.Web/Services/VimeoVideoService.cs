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
    /// This service contains logic specific to querying the Vimeo API to return a video and it's data
    /// Examples of its use can be found at: 
    /// http://developer.vimeo.com/apis/simple#activity-response-example
    /// </summary>
    public class VimeoVideoService : VideoServiceBase, IVimeoVideoService
    {

        #region Members

        private Logger _logger = LogManager.GetLogger("VimeoVideoService");

        private readonly IDocumentSession _documentSession;

        private const string _uriFormat = @"http://player.vimeo.com/video/{0}";
        private const string _apiUriFormat = @"http://vimeo.com/api/v2/video/{0}.json";

        #endregion

        #region Constructors

        public VimeoVideoService(
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
            if (!_documentSession.Load<AppRoot>(Constants.AppRootId).VimeoVideoServiceStatus)
            {
                failureReason = "YouTube video files cannot be imported at the moment. Please try again later.";
                return false;
            }

            try
            {
                string apiUri = string.Format(_apiUriFormat, command.VideoId);

                dynamic data = GetVideoDataFromApi(apiUri);

                //                   {"title","Title"},
                //                   {"url","Url"},
                //                   {"id","Id"},
                //                   {"upload_date","Uploaded"},
                //                   {"thumbnail_small","Title"},
                //                   {"thumbnail_medium","Title"},
                //                   {"thumbnail_large","Title"},
                //                   {"duration","Title"},
                //                   {"width","Title"},
                //                   {"height","Title"}
                AddMetadata(mediaResource, "vimeo", command.VideoId);

                MakeVideoMediaResourceFiles(mediaResource, data, string.Format(_uriFormat, command.VideoId), "vimeo", command.VideoId);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving video", exception);

                failureReason = "The video cannot be retrieved from YouTube. Please check the video and try again.";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }     

        #endregion

    }
}