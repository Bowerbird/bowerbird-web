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
using System.Net;
using Bowerbird.Core.Utilities;
using Newtonsoft.Json;

namespace Bowerbird.Web.Utilities
{
    /// <summary>
    /// This provider utility contains logic specific to querying the Vimeo API to return a video and it's data
    /// Examples of its use can be found at: 
    /// http://developer.vimeo.com/apis/simple#activity-response-example
    /// </summary>
    public class VimeoVideoUtilityProvider : IVideoUtilityProvider
    {
        private List<string> _providerDetection;
        private const string _srcTag = @"http://player.vimeo.com/video/";
        private const string _apiUrl = @"http://vimeo.com/api/v2/video/{0}.json";

        public VimeoVideoUtilityProvider()
        {
            InitMembers();
        }

        public string Name()
        {
            return "Vimeo";
        }

        public string SrcTag()
        {
            return _srcTag;
        }

        public bool IsMatch(string url)
        {
            return _providerDetection.Any(x => url.Contains(x));
        }

        public Dictionary<string, string> VideoData()
        {
            return new Dictionary<string, string>()
                       {
                           {"title","Title"},
                           {"url","Url"},
                           {"id","Id"},
                           {"upload_date","Uploaded"},
                           {"thumbnail_small","Title"},
                           {"thumbnail_medium","Title"},
                           {"thumbnail_large","Title"},
                           {"duration","Title"},
                           {"width","Title"},
                           {"height","Title"}
                       };
        }

        /// <summary>
        /// Vimeo links always seem to end in a slash without any querystring tokens
        /// </summary>
        public string VideoId(string url)
        {
            return url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/") - 1);
        }

        /// <summary>
        /// Return a url to make a video metadata request for
        /// </summary>
        public string VideoDataUrl(string videoId)
        {
            return string.Format(_apiUrl, videoId);
        }

        public dynamic GetVideoDataFromApi(string apiCall)
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
                    catch (Exception ex)
                    {
                        apiRequestCount++;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Initialize with all known provider url patterns
        /// </summary>
        private void InitMembers()
        {
            _providerDetection = new List<string>() { "vimeo.com",  };
        }
    }
}