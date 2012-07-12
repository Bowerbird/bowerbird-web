/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.VideoUtilities
{
    public interface IVideoUtility
    {
        bool PreviewVideoTag(string videoUrl, out string preview);

        bool IsValidVideo(string url, out string embedScript, out string videoId, out string provider);
    }
}