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

namespace Bowerbird.Core.Utilities
{
    internal interface IVideoUtilityProvider
    {
        string Name();

        bool IsMatch(string url);

        string VideoId(string url);

        string SrcTag();

        string VideoDataUrl(string videoId);

        Dictionary<string, string> VideoData();
    }
}