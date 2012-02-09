/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using System;

namespace Bowerbird.Core.DomainModels
{
    public class Post : Contribution
    {
        #region Members

        private List<Comment> _comments;

        private List<MediaResource> _mediaResources;

        #endregion

        #region Constructors

        protected Post() : base() { }

        public Post(
            User createdByUser,
            DateTime createdOn,
            string subject,
            string message,
            IEnumerable<MediaResource> mediaResources,
            Group group)
            : base(
            createdByUser,
            createdOn)
        {
            Check.RequireNotNullOrWhitespace(subject, "subject");
            Check.RequireNotNullOrWhitespace(message, "message");
            Check.RequireNotNull(mediaResources, "mediaResources");
            Check.RequireNotNull(group, "group");

            InitMembers();

            SetDetails(
                subject,
                message,
                mediaResources
                );

            AddGroupContribution(group, createdByUser, createdOn);

            EventProcessor.Raise(new DomainModelCreatedEvent<Post>(this, createdByUser));
        }

        #endregion

        #region Properties

        public string Subject { get; private set; }

        public string Message { get; private set; }

        public IEnumerable<Comment> Comments { get { return _comments; } }

        public IEnumerable<MediaResource> MediaResources { get { return _mediaResources; } }

        #endregion

        #region Methods

        private void SetDetails(string subject, string message, IEnumerable<MediaResource> mediaResources)
        {
            Subject = subject;
            Message = message;
            _mediaResources = mediaResources.ToList();
        }

        public Post UpdateDetails(User updatedByUser, string subject, string message, IEnumerable<MediaResource> mediaResources)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                subject,
                message,
                mediaResources);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Post>(this, updatedByUser));

            return this;
        }

        public void AddComment(Comment comment, User createdByUser, DateTime createdDateTime)
        {
            Check.RequireNotNull(comment, "comment");
            Check.RequireNotNull(createdByUser, "createdByUser");

            _comments.Add(comment);

            EventProcessor.Raise(new DomainModelCreatedEvent<Comment>(comment, createdByUser));
        }

        public void RemoveComment(string commentId)
        {
            if (_comments.Any(x => x.Id == commentId))
            {
                _comments.RemoveAll(x => x.Id == commentId);
            }
        }

        private void InitMembers()
        {
            _comments = new List<Comment>();

            _mediaResources = new List<MediaResource>();
        }

        #endregion
    }
}