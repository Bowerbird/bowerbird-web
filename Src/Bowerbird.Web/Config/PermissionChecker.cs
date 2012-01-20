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
using Bowerbird.Core.DomainModels.Members;
using Raven.Client;

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
            _cachedRoles = _documentSession.Query<Role>();
        }

        public bool HasGlobalPermission(string userId, string permissionId)
        {
            var globalMember = _documentSession.Load<GlobalMember>(userId);

            return HasPermission(globalMember, permissionId);
        }

        public bool HasTeamPermission(string userId, string teamId, string permissionId)
        {
            var teamMember = _documentSession.Load<TeamMember>(teamId);

            return HasPermission(teamMember, permissionId);
        }

        public bool HasProjectPermission(string userId, string projectId, string permissionId)
        {
            var projectMember = _documentSession.Load<ProjectMember>(projectId);

            return HasPermission(projectMember, permissionId);
        }

        public bool HasPermissionToUpdate<T>(string userId, string id)
        {
            if(default(T) is Observation)
            {
                // HACK: Temp implementation until we see a pattern emerge in the usage of permissions
                var observation = _documentSession.Load<Observation>(id);
                return observation.User.Id == userId;
            }

            throw new ArgumentException("The specified model type does not have a permission check implemented.");
        }

        public bool HasPermissionToDelete<T>(string userId, string id)
        {
            if (default(T) is Observation)
            {
                // HACK: Temp implementation until we see a pattern emerge in the usage of permissions
                var observation = _documentSession.Load<Observation>(id);
                return observation.User.Id == userId;
            }

            throw new ArgumentException("The specified model type does not have a permission check implemented.");
        }

        private bool HasPermission(Member member, string permissionId)
        {
            if (_cachedRoles == null)
            {
                throw new Exception("PermissionChecker has not been initialised. Call Init() before use.");
            }

            return _cachedRoles
                .Where(x => member.Roles.Any(y => y.Id == x.Id))
                .Any(x => x.Permissions.Any(y => y.Id == permissionId));
        }

        #endregion      
      
    }
}
