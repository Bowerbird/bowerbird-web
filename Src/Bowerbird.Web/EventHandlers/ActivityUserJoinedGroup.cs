﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

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
        IEventHandler<DomainModelCreatedEvent<Team>>,
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
                        Execute(domainEvent, groupResult.Group, groupResult.UserIds.Count(), groupResult.ObservationIds.Count(), groupResult.PostIds.Count(), domainEvent.Sender as User);
                    }
                }
            }
        }

        public void Handle(DomainModelCreatedEvent<Project> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, 1, 0, 0, domainEvent.User);
        }

        public void Handle(DomainModelCreatedEvent<Team> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, 1, 0, 0, domainEvent.User);
        }

        public void Handle(DomainModelCreatedEvent<Organisation> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, 1, 0, 0, domainEvent.User);
        }

        private void Execute(IDomainEvent domainEvent, Group group, int memberCount, int observationCount, int postCount, User newMember)
        {
            var user = _documentSession.Load<User>(newMember.Id);

            foreach (var session in user.Sessions)
            {
                _backChannelService.AddUserToGroupChannel(group.Id, session.ConnectionId);
            }

            var groupViewModel = _groupViewFactory.Make(group, memberCount, observationCount, postCount);

            _backChannelService.SendJoinedGroupToUserChannel(user.Id, groupViewModel);

            dynamic activity = MakeActivity(
                domainEvent,
                "userjoinedgroup",
                string.Format("{0} joined {1}", user.FirstName, group.Name),
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