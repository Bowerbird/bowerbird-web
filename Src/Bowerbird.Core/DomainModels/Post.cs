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

namespace Bowerbird.Core.DomainModels
{
    public class Post : DomainModel, IOwnable, IDiscussable, IContribution
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<MediaResource> _mediaResources;

        #endregion

        #region Constructors

        protected Post()
            : base()
        {
            InitMembers();
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

            SetPostDetails(
                subject,
                message,
                mediaResources
                );

            ApplyEvent(new DomainModelCreatedEvent<Post>(this, createdByUser, this));
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

        public Discussion Discussion { get; private set; }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        IEnumerable<string> IOwnable.Groups
        {
            get { return new string[] { this.Group.Id }; }
        }

        #endregion

        #region Methods

        private void SetPostDetails(string subject, string message, IEnumerable<MediaResource> mediaResources)
        {
            Subject = subject;
            Message = message;
            _mediaResources = mediaResources.ToList();
        }

        public Post UpdateDetails(User updatedByUser, string subject, string message, IEnumerable<MediaResource> mediaResources)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetPostDetails(
                subject,
                message,
                mediaResources);

            ApplyEvent(new DomainModelUpdatedEvent<Post>(this, updatedByUser, this));

            return this;
        }

        private void InitMembers()
        {
            Discussion = new Discussion();
            _mediaResources = new List<MediaResource>();
        }

        public ISubContribution GetSubContribution(string type, string id)
        {
            return null;
        }

        #endregion
    }
}