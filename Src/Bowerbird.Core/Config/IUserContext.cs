﻿/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
    public interface IUserContext
    {

        bool IsUserAuthenticated();

        string GetAuthenticatedUserId();

        bool HasEmailCookieValue();

        string GetEmailCookieValue();

        void SignUserIn(string userId, string email, bool keepUserLoggedIn);

        void SignUserOut();

        /// <summary>
        /// Determine if user has the specified permission to perform a task on the app root level
        /// </summary>
        /// <param name="permissionId">The permission name to find</param>
        bool HasAppRootPermission(string permissionId);

        /// <summary>
        /// Ensure user domain model object is being edited by the actual user
        /// </summary>
        /// <returns></returns>
        bool HasUserPermission(string domainModelId);

        /// <summary>
        /// Determine if user has the specified permission to perform a task on their user project
        /// </summary>
        /// <param name="permissionId">The permission name to find</param>
        bool HasUserProjectPermission(string permissionId);

        /// <summary>
        /// Determine if user has the specified permission to perform a task in the specified group
        /// </summary>
        /// <param name="permissionId">The permission name to find</param>
        /// <param name="groupId">The group within which to query for the specified permission</param>
        bool HasGroupPermission(string permissionId, string groupId);

        /// <summary>
        /// Determine if user has the specified permission to perform a task in the specified group on the specified domain model
        /// </summary>
        /// <typeparam name="T">The type of domain model that permission is being queried for</typeparam>
        /// <param name="permissionId">The permission name to find</param>
        /// <param name="domainModelId">The domain model to check for permission</param>
        bool HasGroupPermission<T>(string permissionId, string domainModelId) where T : IOwnable;

        /// <summary>
        /// Determine if user has the specified role in the specified group
        /// </summary>
        bool HasRole(string roleId, string groupId);
    }
}
