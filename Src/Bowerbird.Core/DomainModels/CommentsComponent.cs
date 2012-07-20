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
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.DomainModels
{
    public class CommentsComponent
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<Comment> _comments;

        #endregion

        #region Constructors

        public CommentsComponent()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public IEnumerable<Comment> Comments 
        {
            get { return _comments; }
            private set { _comments = new List<Comment>(value); } 
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _comments = new List<Comment>();
        }

        /// <summary>
        /// This Method is to add a comment at the root level of a contribution.
        /// These comments will have the id 1, 2, 3, 4...
        /// </summary>
        public Comment AddComment(
            string message, 
            User createdByUser, 
            DateTime createdDateTime,
            string contributionId)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
          
            var commentId = "1";

            var comments = _comments
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .ToList();

            if (comments.IsNotNullAndHasItems())
            {
                int idOfLastComment;
                if (Int32.TryParse(comments[comments.Count - 1], out idOfLastComment))
                {
                    commentId = (++idOfLastComment).ToString();
                }
            }

            var newComment = new Comment(commentId, createdByUser, createdDateTime, message, contributionId);

            _comments.Add(newComment);

            return newComment;
        }

        public void RemoveComment(string commentId)
        {
            _comments.RemoveAll(x => x.Id == commentId);
        }

        public void UpdateComment(string commentId, string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            if (_comments.Any(x => x.Id == commentId))
            {
                var comment = _comments.Where(x => x.Id == commentId).FirstOrDefault();

                comment.UpdateDetails(modifiedByUser, modifiedDateTime, message);
            }
        }

        /// <summary>
        /// This method is to add a comment to a comment.
        /// These comments will have the id 1.1, 1.1.1, 1.1.2, 2.1, 2.2....
        /// </summary>
        public Comment AddThreadedComment(string message,
            User createdByUser,
            DateTime createdDateTime,
            string parentCommentId,
            string contributionId
            )
        {
            var parentComment = FindParentComment(parentCommentId);

            Check.RequireNotNull(parentComment, "parentComment");

            var siblingCount = parentComment.Comments != null ? parentComment.Comments.Count : 0;

            var commentId = string.Format("{0}.{1}", parentCommentId, ++siblingCount);

            return parentComment.AddComment(new Comment(commentId, createdByUser, createdDateTime, message, contributionId, true));
        }

        private Comment FindParentComment(string parentCommentId)
        {
            var findParentComment = new Stack<string>(parentCommentId.Split('.'));

            Comment comment = null;
            int index;

            // get the root comment from the first token of the parent comment id
            if (ElementAt(findParentComment.Pop(), out index))
            {
                comment = Comments.ElementAt(index);
            }

            while (comment != null && findParentComment.Count > 0)
            {
                if (ElementAt(findParentComment.Pop(), out index))
                {
                    comment = comment.Comments.ElementAt(index);
                }
            }

            return comment;
        }

        /// <summary>
        /// Taking a string Id of 4.1.2, we need to find the elementAt of [3][0][1]
        /// </summary>
        private static bool ElementAt(string id, out int elementAt)
        {
            if (int.TryParse(id, out elementAt))
            {
                elementAt = elementAt - 1;

                return elementAt >= 0;
            }

            return false;
        }

        #endregion
    }
}