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
    internal interface IVideoUtilityProvider
    {
        string Name();

        bool IsMatch(string url);

        string VideoId(string url);

        string SrcTag();
    }
}