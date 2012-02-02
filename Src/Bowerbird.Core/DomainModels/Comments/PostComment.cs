/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels.Comments
{
    public class PostComment : Comment
    {
        #region Members

        #endregion

        #region Constructors

        protected PostComment() : base()
        {
        }

        public PostComment(
            User createdByUser, 
            Post post, 
            DateTime commentedOn,
            string message)
            : base(createdByUser,
            commentedOn,
            message)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            Post = post;

            EventProcessor.Raise(new DomainModelCreatedEvent<PostComment>(this, createdByUser));
        }

        #endregion

        #region Properties

        public Post Post { get; private set; }

        #endregion

        #region Methods

        public PostComment UpdateCommentDetails(
            User updatedByUser,
            DateTime updatedOn,
            string message
            )
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            UpdateDetails(
                updatedByUser,
                updatedOn,
                message
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<PostComment>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}