/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Config
{
    public interface IPermissionChecker
    {
        bool HasGlobalPermission(string userId, string permissionName);

        bool HasGroupPermission(string userId, string groupId, string permissionName);

        bool HasProjectObservationDeletePermission(string userId, string observationId, string projectId);

        bool HasPermissionToUpdate<T>(string userId, string id);

        bool HasPermissionToDelete<T>(string userId, string id);
    }
}