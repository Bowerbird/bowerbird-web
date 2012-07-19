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
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class Post : DomainModel, IOwnable, IContribution, IDiscussed
    {
        #region Members

        [JsonIgnore]
        private List<MediaResource> _mediaResources;

        #endregion

        #region Constructors

        protected Post()
            : base()
        {
            InitMembers();

            EnableEvents();
        }

        public Post(
            User createdByUser,
            DateTime createdOn,
            string subject,
            string message,
            IEnumerable<MediaResource> mediaResources,
            Group group)
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(subject, "subject");
            Check.RequireNotNullOrWhitespace(message, "message");
            Check.RequireNotNull(mediaResources, "mediaResources");
            Check.RequireNotNull(group, "group");

            InitMembers();

            User = createdByUser;
            CreatedOn = createdOn;
            Group = group;

            SetDetails(
                subject,
                message,
                mediaResources
                );

            EnableEvents();

            FireEvent(new DomainModelCreatedEvent<Post>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public string Subject { get; private set; }

        public string Message { get; private set; }

        public IEnumerable<MediaResource> MediaResources 
        { 
            get { return _mediaResources; }
            private set { _mediaResources = new List<MediaResource>(value); } 
        }

        public DenormalisedGroupReference Group { get; private set; }

        public CommentsComponent Discussion { get; private set; }

        [JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return new string[] { this.Group.Id }; }
        }

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

            FireEvent(new DomainModelUpdatedEvent<Post>(this, updatedByUser, this));

            return this;
        }

        IContribution IDiscussed.AddComment(string message, User createdByUser, DateTime createdDateTime)
        {
            var comment = Discussion.AddComment(message, createdByUser, createdDateTime);

            FireEvent(new DomainModelCreatedEvent<Comment>(comment, createdByUser, this));

            return this;
        }

        IContribution IDiscussed.AddThreadedComment(string message, User createdByUser, DateTime createdDateTime, Comment commentToRespondTo)
        {
            var comment = Discussion.AddThreadedComment(message, createdByUser, createdDateTime, commentToRespondTo);

            FireEvent(new DomainModelCreatedEvent<Comment>(comment, createdByUser, this));

            return this;
        }

        IContribution IDiscussed.RemoveComment(string commentId)
        {
            Discussion.RemoveComment(commentId);

            return this;
        }

        IContribution IDiscussed.UpdateComment(string commentId, string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Discussion.UpdateComment(commentId, message, modifiedByUser, modifiedDateTime);

            return this;
        }

        private void InitMembers()
        {
            Discussion = new CommentsComponent();
            _mediaResources = new List<MediaResource>();
        }

        #endregion
    }
}