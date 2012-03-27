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
    public interface IUserContext
    {

        bool IsUserAuthenticated();

        string GetAuthenticatedUserId();

        bool HasEmailCookieValue();

        string GetEmailCookieValue();

        void SignUserIn(string email, bool keepUserLoggedIn);

        void SignUserOut();

        dynamic GetChannel();

        bool HasGlobalPermission(string permissionName);

        bool HasGroupPermission(string groupId, string permissionName);

        bool HasPermissionToUpdate<T>(string id);

        bool HasPermissionToDelete<T>(string id);

    }
}
