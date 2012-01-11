using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class ProjectMember : Member
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
            : base(
            user,
            roles)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(project, "project");            

            SetDetails(project, user);

            EventProcessor.Raise(new DomainModelCreatedEvent<ProjectMember>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedNamedDomainModelReference<Project> Project { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
        }

        private void SetDetails(Project project, User user)
        {
            Project = project;

            base.SetDetails(user);
        }

        #endregion
    }
}