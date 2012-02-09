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
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public abstract class Contribution : DomainModel
    {

        #region Members

        private List<GroupContribution> _groupContributions;

        private List<Comment> _comments;

        #endregion

        #region Constructors

        protected Contribution()
        {
            InitMembers();
        }

        protected Contribution(
            User createdByUser,
            DateTime createdOn) 
            : this() 
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            User = createdByUser;
            CreatedOn = createdOn;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public IEnumerable<GroupContribution> GroupContributions { get { return _groupContributions; } }

        public IEnumerable<Comment> Comments { get { return _comments; } }

        #endregion

        #region Methods

        public void AddGroupContribution(Group group, User createdByUser, DateTime createdDateTime)
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(createdByUser, "createdByUser");

            if (_groupContributions.All(x => x.GroupId != group.Id))
            {
                var groupContribution = new GroupContribution(group, createdByUser, createdDateTime);

                _groupContributions.Add(groupContribution);

                EventProcessor.Raise(new DomainModelCreatedEvent<GroupContribution>(groupContribution, createdByUser));
            }
        }

        public void RemoveGroupContribution(string groupId)
        {
            if (_groupContributions.Any(x => x.GroupId == groupId))
            {
                _groupContributions.RemoveAll(x => x.GroupId == groupId);
            }
        }

        public void AddComment(string message, User createdByUser, DateTime createdDateTime)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            var newComment = new Comment(createdByUser, createdDateTime, message);

            _comments.Add(newComment);

            EventProcessor.Raise(new DomainModelCreatedEvent<Comment>(newComment, createdByUser));
        }

        public void AddComment(string commentId)
        {
            if (_comments.Any(x => x.Id == commentId))
            {
                _comments.RemoveAll(x => x.Id == commentId);
            }
        }

        private void InitMembers()
        {
            _comments = new List<Comment>();

            _groupContributions = new List<GroupContribution>();
        }

        #endregion      
      
    }
}
