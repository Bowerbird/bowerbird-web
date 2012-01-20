/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
        bool HasGlobalPermission(string userId, string permissionId);

        bool HasTeamPermission(string userId, string teamId, string permissionId);

        bool HasProjectPermission(string userId, string projectId, string permissionId);

        bool HasPermissionToUpdate<T>(string userId, string id);

        bool HasPermissionToDelete<T>(string userId, string id);
    }
}