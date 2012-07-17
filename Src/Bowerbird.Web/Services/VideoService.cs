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
using System.Dynamic;
using System.Net;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Services;
using Bowerbird.Core.Utilities;
using Newtonsoft.Json;

namespace Bowerbird.Web.Services
{
    public class VideoService : IVideoService
    {
        #region Fields

        private readonly IVideoUtility _videoUtility;

        #endregion

        #region Constructors

        public VideoService(
            IVideoUtility videoUtility
            )
        {
            Check.RequireNotNull(videoUtility, "videoUtility");
            
            _videoUtility = videoUtility;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Save(MediaResourceCreateCommand command, MediaResource mediaResource)
        {
            string provider; // playback service - youtube, vimeo et al
            string videoId; // unique identifier for video on playback service
            string embedString; // the embed html tags with format options for video id and sizes
            string providerApiUrl; // the constructed url to call to download video metadata from provider api

            // hit up the video utility to validate the url of the file
            // and to query the api for the video's metadata
            if (_videoUtility.IsValidVideo(command.VideoUri, out embedString, out videoId, out provider, out providerApiUrl))
            {
                mediaResource
                    .AddMetadata("Url", command.VideoUri)
                    .AddMetadata("Provider", provider)
                    .AddMetadata("VideoId", videoId);

                var data = GetVideoDataFromApi(providerApiUrl);

                ((dynamic) mediaResource).VideoData = data;

                MakeVideoMediaResourceFiles(
                    mediaResource,
                    embedString.AppendWith(videoId),
                    command.VideoUri,
                    provider,
                    videoId);
            }
        }

        private void MakeVideoMediaResourceFiles(MediaResource mediaResource, string embedScript, string linkUri, string provider, string videoId)
        {
            mediaResource.AddVideoFile("Fullsize", linkUri, embedScript, provider, videoId, "672", "400");
            mediaResource.AddVideoFile("Preview", linkUri, embedScript, provider, videoId, "220", "200");
            mediaResource.AddVideoFile("Small", linkUri, embedScript, provider, videoId, "120", "80");
            mediaResource.AddVideoFile("Thumb", linkUri, embedScript, provider, videoId, "60", "40");
        }

        /// <summary>
        /// Using a web client, grab the video data from the service api try and pull the data 3 times before failing.
        /// </summary>
        private dynamic GetVideoDataFromApi(string apiCall)
        {
            const int apiRequestAttempts = 3;

            using(var apiWebClient = new WebClient())
            {
                int apiRequestCount = 1;

                while (apiRequestCount < apiRequestAttempts)
                {
                    try
                    {
                        var data = apiWebClient.DownloadString(apiCall);

                        return JsonConvert.DeserializeObject<dynamic>(data);
                    }
                    catch (Exception ex)
                    {
                        apiRequestCount++;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}