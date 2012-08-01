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
using System.Net;
using Bowerbird.Core.DomainModels;
using NLog;

namespace Bowerbird.Web.Services
{
    public abstract class VideoServiceBase
    {
        #region Fields

        private Logger _logger = LogManager.GetLogger("VideoServiceBase");

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected void AddMetadata(MediaResource mediaResource, string providerName, string videoId)
        {
            mediaResource
                .AddMetadata("Provider", providerName)
                .AddMetadata("VideoId", videoId);
        }

        protected void MakeVideoMediaResourceFiles(MediaResource mediaResource, dynamic data, string uri, string provider, string videoId)
        {
            dynamic original = mediaResource.AddVideoFile("Original", uri, provider, videoId, 1280, 1024);

            original.Data = data;

            mediaResource.AddVideoFile("Square42", uri, provider, videoId, 42, 42);
            mediaResource.AddVideoFile("Square100", uri, provider, videoId, 100, 100);
            mediaResource.AddVideoFile("Square200", uri, provider, videoId, 200, 200);
            mediaResource.AddVideoFile("Full480", uri, provider, videoId, 640, 480);
            mediaResource.AddVideoFile("Full768", uri, provider, videoId, 1024, 768);
            mediaResource.AddVideoFile("Full1024", uri, provider, videoId, 1280, 1024);
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
                        _logger.ErrorException("Error saving images", exception);

                        apiRequestCount++;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}