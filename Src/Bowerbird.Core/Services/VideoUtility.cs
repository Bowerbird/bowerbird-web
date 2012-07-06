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
using System.IO;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Config;
using Bowerbird.Core.Extensions;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Bowerbird.Core.Services
{
    public class VideoUtility : IVideoUtility
    {
        #region Members

        private List<IVideoProvider> _providers { get; set; }

        private string _errorMessage { get; set; }

        #endregion

        #region Constructors

        public VideoUtility()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool PreviewVideoTag(string videoUrl, out string display)
        {
            var videoProvider = _providers.Where(x => x.IsMatch(videoUrl)).FirstOrDefault();

            if (videoProvider != null)
            {
                display = videoProvider.PreviewVideoTag(videoProvider.VideoId(videoUrl));

                return true;
            }
            else
            {
                display = _errorMessage;

                return false;
            }
        }

        public bool IsValidVideo(string url, out string embedScript, out string videoId, out string provider)
        {
            embedScript = videoId = provider = string.Empty;

            var videoProvider = _providers.Where(x => x.IsMatch(url)).FirstOrDefault();

            if (videoProvider != null)
            {
                embedScript = videoProvider.EmbedScript();
                videoId = videoProvider.VideoId(url);
                provider = videoProvider.Name();

                return true;
            }

            return false;
        }

        public string VideoId(string videoUrl)
        {
            var videoProvider = _providers.Where(x => x.IsMatch(videoUrl)).FirstOrDefault();

            if (videoProvider != null)
            {
                return videoProvider.VideoId(videoUrl);
            }
            else
            {
                return _errorMessage;
            }
        }

        public string Provider(string videoUrl)
        {
            var videoProvider = _providers.Where(x => x.IsMatch(videoUrl)).FirstOrDefault();

            if (videoProvider != null)
            {
                return videoProvider.Name();
            }
            else
            {
                return _errorMessage;
            }
        }

        private void InitMembers()
        {
            _providers = new List<IVideoProvider>(){new YoutubeVideoProvider(), new VimeoVideoProvider()};

            _errorMessage = string.Format("Your video is not from our listed providers, {0}", _providers.Select(x => x.Name()).ToCommaString());
        }

        #endregion

        internal interface IVideoProvider
        {
            string Name ();

            bool IsMatch(string url);
            
            string VideoId(string url);

            string PreviewVideoTag(string videoId, string height = Default.MovieHeight, string width = Default.MovieWidth);

            string EmbedScript();
        }

        private class YoutubeVideoProvider : IVideoProvider
        {
            private List<string> _providerDetection;
            private const string _embedTag = @"<iframe width='{0}' height='{1}' src='http://www.youtube.com/embed/{2}' frameborder='0' allowfullscreen></iframe>";
        
            public YoutubeVideoProvider()
            {
                InitMembers();
            }

            public string Name()
            {
	            return "Youtube";
            }

            public string EmbedScript()
            {
                return _embedTag;
            }

            public string PreviewVideoTag(string videoId, string height = Default.MovieHeight, string width = Default.MovieWidth)
            {
                return string.Format(_embedTag, width, height, videoId);
            }

            public bool IsMatch(string url)
            {
                return _providerDetection.Any(x => url.Contains(x));
            }

            // youtube links can be route style or querystring which can have parameters at the end.
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
                                                                   
                    idString = idString.Substring( location + 1, length - location - 1);
                    
                    // EG: www.youtube.com/watch?v=XMa96U5lsjk&feature=g-vrec
                    if(idString.Contains("&"))
                    { 
                        // strip additional parameter data
                        idString = idString.Substring( 0, idString.IndexOf("&"));
                    }
                }

                return idString;
            }

            private void InitMembers()
            {
                _providerDetection = new List<string>(){"www.youtube.com","youtu.be"};
            }
        }

        private class VimeoVideoProvider : IVideoProvider
        {
            private List<string> _providerDetection;
            private const string _embedTag = @"<iframe width='{0}' height='{1}' src='http://player.vimeo.com/video/{3}'frameborder='0' allowFullScreen></iframe>";
        
            public VimeoVideoProvider()
            {
                InitMembers();
            }

            public string Name()
            {
	            return "Vimeo";
            }

            public string EmbedScript()
            {
                return _embedTag;
            }

            public string PreviewVideoTag(string videoId, string height = Default.MovieHeight, string width = Default.MovieWidth)
            {
                return string.Format(_embedTag, width, height, videoId);
            }

            public bool IsMatch(string url)
            {
                return _providerDetection.Any(x => url.Contains(x));
            }

            // Vimeo links always seem to end in a slash without any querystring tokens
            public string VideoId(string url)
            {
 	            return url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/"));
            }

            private void InitMembers()
            {
                _providerDetection = new List<string>(){"www.vimeo.com"};
            }
        }
    }
}