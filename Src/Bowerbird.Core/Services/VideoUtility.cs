/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Extensions;
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

        public bool PreviewVideoTag(string videoUrl, out string preview)
        {
            var videoProvider = _providers.Where(x => x.IsMatch(videoUrl)).FirstOrDefault();

            if (videoProvider != null)
            {
                preview = videoProvider.SrcTag().AppendWith(videoProvider.VideoId(videoUrl));

                return true;
            }
            else
            {
                preview = _errorMessage;

                return false;
            }
        }

        public bool IsValidVideo(string url, out string srcTag, out string videoId, out string provider)
        {
            srcTag = videoId = provider = string.Empty;

            var videoProvider = _providers.Where(x => x.IsMatch(url)).FirstOrDefault();

            if (videoProvider != null)
            {
                srcTag = videoProvider.SrcTag();
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

            string SrcTag();
        }

        private class YoutubeVideoProvider : IVideoProvider
        {
            private List<string> _providerDetection;
            private const string _srcTag = @"http://www.youtube.com/embed/";
        
            public YoutubeVideoProvider()
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
            private const string _srcTag = @"http://player.vimeo.com/video/";
        
            public VimeoVideoProvider()
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