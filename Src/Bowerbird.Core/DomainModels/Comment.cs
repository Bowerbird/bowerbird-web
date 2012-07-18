/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Comment : DomainModel, IAssignableId
    {
        #region Members

        #endregion

        #region Constructors

        protected Comment() : base() 
        {
            EnableEvents();
        }

        public Comment(
            string commentId,
            User createdByUser,
            DateTime commentedOn,
            string message,
            bool isNested = false
            )
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(commentId, "commentId");
            Check.RequireNotNullOrWhitespace(message, "message");

            CommentedOn = commentedOn;
            User = createdByUser;
            Id = commentId;
            IsNested = isNested;

            SetDetails(
                message,
                CommentedOn);

            EnableEvents();
        }

        #endregion

        #region Properties

        public List<Comment> ThreadedComments { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public DateTime CommentedOn { get; private set; }

        public DateTime EditedOn { get; private set; }

        public string Message { get; private set; }

        public bool IsNested { get; set; }

        #endregion

        #region Methods

        private void SetDetails(string message, DateTime editedOn)
        {
            Message = message;
            EditedOn = editedOn;
        }

        public Comment UpdateDetails(User updatedByUser, DateTime editedOn, string message)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                message,
                editedOn);

            return this;
        }

        public Comment AddThreadedComment(Comment comment)
        {
            ThreadedComments.Add(comment);

            return comment;
        }

        #endregion      

        void IAssignableId.SetIdTo(string prefix, string assignedId)
        {
            Id = string.Format("{0}/{1}", prefix, assignedId);
        }
    }
}