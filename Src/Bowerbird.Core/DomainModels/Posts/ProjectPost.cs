using System;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels.Posts
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
            DateTime timestamp,
            string subject,
            string message,
            IList<MediaResource> mediaResources
            )
            : base(createdByUser,
            timestamp,
            subject,
            message,
            mediaResources)
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