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

using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Web.Config
{
    public class PermissionChecker : IPermissionChecker
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        private IEnumerable<Role> _cachedRoles;

        #endregion

        #region Constructors

        public PermissionChecker(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Init()
        {
            _cachedRoles = _documentSession.Query<Role>(); // HACK: If we have too many roles, RavenDB won't return them all
        }

        public bool HasGroupPermission(string permissionId, string userId, string groupId)
        {
            Check.Ensure(_cachedRoles != null, "PermissionChecker has not been initialised. Call Init() before use.");

            var membership = _documentSession.LoadMember(groupId, userId);

            return _cachedRoles
                .Where(x => membership.Roles.Any(y => y.Id == x.Id))
                .Any(x => x.Permissions.Any(y => y.Id == "permissions/" + permissionId));
        }

        public bool HasGroupPermission<T>(string permissionId, string userId, string domainModelId)
            where T : DomainModel
        {
            Check.Ensure(_cachedRoles != null, "PermissionChecker has not been initialised. Call Init() before use.");

            var memberships = _documentSession.Query<Member>().Where(x => x.User.Id == userId);

            T domainModel = _documentSession.Load<T>(domainModelId);

            if (domainModel is IOwnable)
            {
                // 1. Check if user is owner
                if ((((IOwnable)domainModel).User.Id == userId))
                {
                    return true;
                }

                // 2. Check if user has a valid permission

                // 2a. Get all roles that will be checked for permissions. Only memberships that the model also has will be searched
                var validRoles = memberships.Where(x => ((IOwnable)domainModel).Groups.Any(y => x.Group.Id == y)).SelectMany(x => x.Roles);

                // 2b. Get all permissions that will be searched
                var permissionsToSearch = _cachedRoles.Where(x => validRoles.Any(y => y.Id == x.Id)).SelectMany(x => x.Permissions);

                // 2c. Search for permission
                return permissionsToSearch.Any(x => "permissions/" + permissionId.ToLower() == x.Id);
            }

            throw new ArgumentException("The specified model is not configured to be checked for permissions (must implement IOwnable).");
        }

        #endregion      
      
    }
}
