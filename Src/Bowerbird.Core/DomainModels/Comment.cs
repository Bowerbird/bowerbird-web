using System;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public abstract class Comment : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected Comment() : base() { }

        protected Comment(
            User createdByUser,
            DateTime commentedOn,
            string message)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(message, "message");

            CommentedOn = commentedOn;
            User = createdByUser;

            SetDetails(
                message,
                CommentedOn);

            //EventProcessor.Raise(new DomainModelCreatedEvent<Comment>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CommentedOn { get; private set; }

        public DateTime EditedOn { get; private set; }

        public string Message { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string message, DateTime editedOn)
        {
            Message = message;
            EditedOn = editedOn;
        }

        protected Comment UpdateDetails(User updatedByUser, DateTime editedOn, string message)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                message,
                editedOn);

            //EventProcessor.Raise(new DomainModelUpdatedEvent<Comment>(this, updatedByUser));

            return this;
        }

        #endregion      
    }
}