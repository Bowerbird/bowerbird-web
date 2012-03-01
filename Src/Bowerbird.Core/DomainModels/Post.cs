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
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.DomainModels
{
    public class Post : Contribution
    {
        #region Members

        //private List<Comment> _comments;

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

            var eventMessage = createdByUser.GetName().AppendWith(" added ").AppendWith(Subject).AppendWith(" post");

            EventProcessor.Raise(new DomainModelCreatedEvent<Post>(this, createdByUser, eventMessage));
        }

        #endregion

        #region Properties

        public string Subject { get; private set; }

        public string Message { get; private set; }

        public IEnumerable<MediaResource> MediaResources { get { return _mediaResources; } }

        public IEnumerable<Comment> Comments
        {
            get { return _comments; }

            private set { _comments = value as List<Comment>; }
        }

        #endregion

        #region Methods

        public override string ContributionTitle()
        {
            return Subject;
        }

        public override string ContributionType()
        {
            return "Post";
        }

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

            var eventMessage = updatedByUser.GetName().AppendWith(" updated the ").AppendWith(Subject).AppendWith(" post");

            EventProcessor.Raise(new DomainModelUpdatedEvent<Post>(this, updatedByUser, eventMessage));

            return this;
        }

        private void InitMembers()
        {
            _comments = new List<Comment>();

            _mediaResources = new List<MediaResource>();
        }

        #endregion
    }
}