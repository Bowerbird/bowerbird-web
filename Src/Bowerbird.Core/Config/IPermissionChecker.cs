/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
namespace Bowerbird.Core.Config
{
    public interface IPermissionChecker
    {
        bool HasGroupPermission(string permissionId, string userId, string groupId);

        bool HasGroupPermission<T>(string permissionId, string userId, string domainModelId) where T : IOwnable;
    }
}