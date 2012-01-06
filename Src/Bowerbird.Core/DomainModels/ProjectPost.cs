using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class ProjectPost : Post
    {
        #region Members

        #endregion

        #region Constructors

        protected ProjectPost() : base() { }

        public ProjectPost(
            Project project,
            User createdByUser,
            string subject,
            string message
            )
            : base(createdByUser,
            subject,
            message)
        {
            Check.RequireNotNull(project, "project");

            Project = project;

            EventProcessor.Raise(new DomainModelCreatedEvent<ProjectPost>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedNamedDomainModelReference<Project> Project { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}