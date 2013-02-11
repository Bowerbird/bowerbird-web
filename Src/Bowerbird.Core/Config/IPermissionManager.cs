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
    public interface IPermissionManager
    {
        /// <summary>
        /// Returns whether user has the specified permission within the specified group
        /// </summary>
        bool HasGroupPermission(string permissionId, string userId, string groupId);

        /// <summary>
        /// Returns whether user has the specified permission within the specified group for the specified IOwnable model
        /// </summary>
        /// <typeparam name="T">The type of IOwnable to check permission for</typeparam>
        bool HasGroupPermission<T>(string permissionId, string userId, string domainModelId) where T : IOwnable;

        /// <summary>
        /// Returns whether the user has the specified role within the specified group
        /// </summary>
        bool HasRole(string userId, string roleId, string groupId);

        bool DoesExist<T>(string id);
    }
}