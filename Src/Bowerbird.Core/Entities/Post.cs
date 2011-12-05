using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using System;
using Bowerbird.Core.Entities.DenormalisedReferences;

namespace Bowerbird.Core.Entities
{
    public class Post : Entity
    {

        #region Members

        #endregion

        #region Constructors

        protected Post() : base() { }

        public Post(
            User createdByUser,
            string subject,
            string message)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(subject, "subject");
            Check.RequireNotNullOrWhitespace(message, "message");

            User = createdByUser;
            SubmittedOn = DateTime.Now;

            SetDetails(
                subject,
                message);

            EventProcessor.Raise(new EntityCreatedEvent<Post>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime SubmittedOn { get; private set; }

        public string Subject { get; private set; }

        public string Message { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string subject, string message)
        {
            Subject = subject;
            Message = message;
        }

        public Post UpdateDetails(User updatedByUser, string subject, string message)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                subject,
                message);

            EventProcessor.Raise(new EntityUpdatedEvent<Post>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}