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
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.ViewModelFactories;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Log an activity item when a user joins a group
    /// </summary>
    public class ActivityUserJoinedGroup : 
        DomainEventHandlerBase, 
        IEventHandler<MemberCreatedEvent>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public ActivityUserJoinedGroup(
            IDocumentSession documentSession,
            IGroupViewFactory groupViewFactory,
            IUserViewFactory userViewFactory,
            IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _groupViewFactory = groupViewFactory;
            _userViewFactory = userViewFactory;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MemberCreatedEvent domainEvent)
        {
            // Do not record an activity for user's in userproject
            if (domainEvent.DomainModel.Group.GroupType != "favourites") 
            {
                Execute(domainEvent, domainEvent.Group, domainEvent.Sender as User, domainEvent.DomainModel.Created);
            }
        }

        private void Execute(IDomainEvent domainEvent, Group group, User newMember, DateTime joined)
        {
            foreach (var session in newMember.Sessions)
            {
                _backChannelService.AddUserToGroupChannel(group.Id, session.ConnectionId);
            }

            var groupViewModel = _groupViewFactory.Make(group, newMember);

            _backChannelService.SendJoinedGroupToUserChannel(newMember.Id, groupViewModel);

            dynamic activity = MakeActivity(
                domainEvent,
                "userjoinedgroup",
                joined,
                string.Format("{0} joined {1}", newMember.Name, group.Name),
                new[] { group });

            activity.UserJoinedGroup = new
            {
                User = _userViewFactory.Make(newMember, null),
                Group = groupViewModel
            };

            _documentSession.Store(activity);
            _documentSession.SaveChanges();
        }

        #endregion

    }
}