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
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Factories;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Log an activity item when a user joins a group
    /// </summary>
    public class ActivityUserJoinedGroup : 
        DomainEventHandlerBase, 
        IEventHandler<MemberCreatedEvent>,
        IEventHandler<DomainModelCreatedEvent<Project>>,
        IEventHandler<DomainModelCreatedEvent<Organisation>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public ActivityUserJoinedGroup(
            IDocumentSession documentSession,
            IGroupViewFactory groupViewFactory,
            IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _groupViewFactory = groupViewFactory;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MemberCreatedEvent domainEvent)
        {
            if (domainEvent.DomainModel.Group.GroupType != "userproject") // Do not record an activity for user's in userproject
            {
                var groupResult = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .Where(x => x.GroupId == domainEvent.Group.Id)
                    .ToList()
                    .FirstOrDefault();

                if (groupResult != null)
                {
                    if (domainEvent.DomainModel.Group.GroupType != "approot")
                    {
                        Execute(domainEvent, groupResult.Group, domainEvent.Sender as User, domainEvent.DomainModel.Created);
                    }
                }
            }
        }

        public void Handle(DomainModelCreatedEvent<Project> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, domainEvent.User, DateTime.UtcNow);
        }

        public void Handle(DomainModelCreatedEvent<Organisation> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, domainEvent.User, DateTime.UtcNow);
        }

        private void Execute(IDomainEvent domainEvent, Group group, User newMember, DateTime joined)
        {
            var user = _documentSession.Load<User>(newMember.Id);

            foreach (var session in user.Sessions)
            {
                _backChannelService.AddUserToGroupChannel(group.Id, session.ConnectionId);
            }

            var groupViewModel = _groupViewFactory.Make(group);

            _backChannelService.SendJoinedGroupToUserChannel(user.Id, groupViewModel);

            dynamic activity = MakeActivity(
                domainEvent,
                "userjoinedgroup",
                joined,
                string.Format("{0} joined {1}", user.Name, group.Name),
                new[] { group });

            activity.UserJoinedGroup = new
            {
                User = user,
                Group = groupViewModel
            };

            _documentSession.Store(activity);
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        #endregion

    }
}