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

namespace Bowerbird.Core.VideoUtilities
{
    public class VideoUtility : IVideoUtility
    {
        #region Members

        private List<IVideoUtilityProvider> _providers { get; set; }

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
            _providers = new List<IVideoUtilityProvider>(){new YoutubeVideoUtilityProvider(), new VimeoVideoUtilityProvider()};

            _errorMessage = string.Format("Your video is not from our listed providers, {0}", _providers.Select(x => x.Name()).ToCommaString());
        }

        #endregion
        
    }
}