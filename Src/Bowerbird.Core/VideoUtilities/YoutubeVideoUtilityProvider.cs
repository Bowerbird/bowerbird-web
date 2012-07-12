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
    internal class YoutubeVideoUtilityProvider : IVideoUtilityProvider
    {
        private List<string> _providerDetection;
        private const string _srcTag = @"http://www.youtube.com/embed/";

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

        private void InitMembers()
        {
            _providerDetection = new List<string>() { "www.youtube.com", "youtu.be" };
        }
    }
}