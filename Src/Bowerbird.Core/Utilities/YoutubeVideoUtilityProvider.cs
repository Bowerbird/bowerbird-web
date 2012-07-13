/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;

namespace Bowerbird.Core.Utilities
{
    /// <summary>
    /// This provider utility contains logic specific to querying the Youtube API to return videos and metadata
    /// Examples of its use can be found at: 
    /// https://developers.google.com/youtube/v3/getting-started
    /// </summary>
    public class YoutubeVideoUtilityProvider : IVideoUtilityProvider
    {
        private List<string> _providerDetection;
        private const string _srcTag = @"http://www.youtube.com/embed/";
        private const string _apiUrl = @"https://www.googleapis.com/youtube/v3alpha/video?id={0}&key={1}&part=id,snippet";
        private const string _apiKey = "c4fa7bdfa198abf2769a5775885409e9ff560546";

        public YoutubeVideoUtilityProvider()
        {
            InitMembers();
        }

        public string Name()
        {
            return "Youtube";
        }

        public string SrcTag()
        {
            return _srcTag;
        }

        public bool IsMatch(string url)
        {
            return _providerDetection.Any(url.Contains);
        }

        public Dictionary<string, string> VideoData()
        {
            return new Dictionary<string, string>()
                       {
                           {"title","Title"},
                           {"description","Description"},
                           {"duration","Duration"},
                           {"aspectRatio","Aspect"}
                       };
        }

        /// <summary>
        /// youtube links can be route style or querystring which can have parameters at the end.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string VideoId(string url)
        {
            // EG: www.youtu.be/XMa96U5lsjk
            var idString = url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/") - 1);

            // EG: www.youtube.com/  watch?v=XMa - index of = is 7. length is 11. length after = is 3
            if (idString.Contains("?"))
            {
                // get the substring from the value side of the first parameter

                var location = idString.IndexOf("=");
                var length = idString.Length;

                idString = idString.Substring(location + 1, length - location - 1);

                // EG: www.youtube.com/watch?v=XMa96U5lsjk&feature=g-vrec
                if (idString.Contains("&"))
                {
                    // strip additional parameter data
                    idString = idString.Substring(0, idString.IndexOf("&"));
                }
            }

            return idString;
        }

        public string ThumbnailUrl(string videoId)
        {
            return "The thumbnail to the video";
        }

        /// <summary>
        /// Return a url to make a video metadata request for in the format:
        /// https: //www.googleapis.com/youtube/v3alpha/video?id=[?? Video Id ??]&key=[?? API Key ??]&part=id,snippet,contentDetails,statistics,status
        /// 
        /// Returns the url to call to retreive a video payload
        /// </summary>
        public string VideoDataUrl(string videoId)
        {
            return string.Format(_apiUrl, videoId, _apiKey);
        }

        /// <summary>
        /// Initialize with all known provider url patterns
        /// </summary>
        private void InitMembers()
        {
            _providerDetection = new List<string>() { "www.youtube.com", "youtu.be" };
        }
    }
}