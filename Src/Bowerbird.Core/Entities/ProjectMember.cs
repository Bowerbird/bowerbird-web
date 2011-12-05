using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;
using System;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Entities
{
    public class ProjectMember : Entity
    {

        #region Members

        #endregion

        #region Constructors

        protected ProjectMember()
            : base()
        {
            InitMembers();
        }

        public ProjectMember(
            User createdByUser,
            Project project,
            User user,
            IEnumerable<Role> roles)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(project, "project");
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(roles, "roles");

            SetDetails(
                project,
                user);

            Roles = roles.Select(x => (DenormalisedNamedEntityReference<Role>)x).ToList();

            EventProcessor.Raise(new EntityCreatedEvent<ProjectMember>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedNamedEntityReference<Project> Project { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public List<DenormalisedNamedEntityReference<Role>> Roles { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Roles = new List<DenormalisedNamedEntityReference<Role>>();
        }

        private void SetDetails(Project project, User user)
        {
            Project = project;
            User = user;
        }

        public ProjectMember AddRole(User updatedByUser, Role role)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNull(role, "role");

            if (Roles.All(x => x.Id != role.Id))
            {
                Roles.Add((DenormalisedNamedEntityReference<Role>)role);
            }

            return this;
        }

        public ProjectMember RemoveRole(User updatedByUser, string roleId)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            if (Roles.Any(x => x.Id == roleId))
            {
                Roles.RemoveAll(x => x.Id == roleId);
            }

            return this;
        }

        #endregion

    }
}