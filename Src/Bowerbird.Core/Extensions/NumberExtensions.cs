/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Core.Extensions
{
    public static class NumberExtensions
    {
        public static int Or(this int number, int @default)
        {
            return number > 0 ? number : @default;
        }
    }
}