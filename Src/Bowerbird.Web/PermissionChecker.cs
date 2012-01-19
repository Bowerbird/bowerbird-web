using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;

namespace Bowerbird.Web
{
    public class PermissionChecker
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly string _userId;

        private IEnumerable<Role> _cachedRoles;

        #endregion

        #region Constructors

        public PermissionChecker(
            IDocumentSession documentSession,
            string userId)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNullOrWhitespace(userId, "userId");

            _documentSession = documentSession;
            _userId = userId;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool HasGlobalPermission(string permissionId)
        {
            var globalMember = _documentSession.Load<GlobalMember>(_userId);

            return HasPermission(globalMember, permissionId);
        }

        public bool HasTeamPermission(string teamId, string permissionId)
        {
            var teamMember = _documentSession.Load<TeamMember>(teamId);

            return HasPermission(teamMember, permissionId);
        }

        public bool HasProjectPermission(string projectId, string permissionId)
        {
            var projectMember = _documentSession.Load<ProjectMember>(projectId);

            return HasPermission(projectMember, permissionId);
        }

        public bool HasPermissionToUpdate<T>(string id)
        {
            if(default(T) is Observation)
            {
                var observation = _documentSession.Load<Observation>(id);
                return observation.User.Id == _userId;
            }

            throw new ArgumentException("The specified model type does not have a permission check implemented.");
        }

        public bool HasPermissionToDelete<T>(string id)
        {
            if (default(T) is Observation)
            {
                var observation = _documentSession.Load<Observation>(id);
                return observation.User.Id == _userId;
            }

            throw new ArgumentException("The specified model type does not have a permission check implemented.");
        }

        private bool HasPermission(Member member, string permissionId)
        {
            if (_cachedRoles == null)
            {
                _cachedRoles = _documentSession.Query<Role>();
            }

            return _cachedRoles
                .Where(x => member.Roles.Any(y => y.Id == x.Id))
                .Any(x => x.Permissions.Any(y => y.Id == permissionId));
        }

        #endregion      
      
    }
}
