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
using Bowerbird.Core.Events;
using Bowerbird.Core.Extensions;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class CommentsComponent
    {
        #region Members

        [JsonIgnore]
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

        public Comment AddComment(string message, User createdByUser, DateTime createdDateTime)
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

            var newComment = new Comment(commentId, createdByUser, createdDateTime, message);

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

        #endregion
    }
}