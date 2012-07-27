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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class CommentNew : CommentBase
    {
        #region Members

        #endregion

        #region Constructors

        protected CommentNew()
            : base()
        {
        }

        public CommentNew(
            User createdByUser,
            DateTime commentedOn,
            string message,
            CommentBase parentComment
            )
            : base(
            parentComment
            )
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(message, "message");

            User = createdByUser;
            CommentedOn = commentedOn;

            SetDetails(
                message,
                createdByUser,
                CommentedOn);
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CommentedOn { get; private set; }

        public DateTime EditedOn { get; private set; }

        public DenormalisedUserReference EditedBy { get; private set; }

        public string Message { get; private set; }

        #endregion

        #region Methods

        protected void SetDetails(string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Message = message;
            EditedBy = modifiedByUser;
            EditedOn = modifiedDateTime;
        }

        public CommentNew UpdateDetails(string id, string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Check.RequireNotNull(modifiedByUser, "modifiedByUser");

            if (Id == id)
            {
                SetDetails(message, modifiedByUser, modifiedDateTime);

                return this;
            }

            foreach (var childComment in Comments)
            {
                var comment = childComment.UpdateDetails(id, message, modifiedByUser, modifiedDateTime);

                if (comment != null)
                {
                    return comment;
                }
            }

            return null;
        }

        #endregion

    }
}