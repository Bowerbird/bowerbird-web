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

namespace Bowerbird.Core.VideoUtilities
{
    internal class VimeoVideoUtilityProvider : IVideoUtilityProvider
    {
        private List<string> _providerDetection;
        private const string _srcTag = @"http://player.vimeo.com/video/";

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

        // Vimeo links always seem to end in a slash without any querystring tokens
        public string VideoId(string url)
        {
            return url.Substring(url.LastIndexOf("/") + 1, url.Length - url.LastIndexOf("/"));
        }

        private void InitMembers()
        {
            _providerDetection = new List<string>() { "www.vimeo.com" };
        }
    }
}