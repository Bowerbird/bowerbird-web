using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Entities
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

            EventProcessor.Raise(new EntityCreatedEvent<ProjectPost>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedNamedEntityReference<Project> Project { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}