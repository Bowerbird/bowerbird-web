/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels.Members
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