using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Post : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected Post() : base() { }

        public Post(
            User createdByUser,
            DateTime timestamp,
            string subject,
            string message,
            IList<MediaResource> mediaResources)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(subject, "subject");
            Check.RequireNotNullOrWhitespace(message, "message");
            Check.RequireNotNull(mediaResources, "mediaResources");

            PostedOn = timestamp;
            User = createdByUser;

            SetDetails(
                subject,
                message,
                mediaResources);

            EventProcessor.Raise(new DomainModelCreatedEvent<Post>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime PostedOn { get; private set; }

        public string Subject { get; private set; }

        public string Message { get; private set; }

        public IList<MediaResource> MediaResources { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string subject, string message, IList<MediaResource> mediaResources)
        {
            Subject = subject;
            Message = message;
            MediaResources = mediaResources;
        }

        public Post UpdateDetails(User updatedByUser, string subject, string message, IList<MediaResource> mediaResources)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                subject,
                message,
                mediaResources);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Post>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}