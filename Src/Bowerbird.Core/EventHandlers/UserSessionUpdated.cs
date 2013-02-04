/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Raven.Client.Linq;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Queries;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Log an activity item when a user joins a group
    /// </summary>
    public class UserSessionUpdated :
        DomainEventHandlerBase,
        IEventHandler<DomainModelCreatedEvent<UserSession>>,
        IEventHandler<DomainModelUpdatedEvent<UserSession>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;
        private readonly IUserViewModelQuery _userViewModelQuery;

        #endregion

        #region Constructors

        public UserSessionUpdated(
            IDocumentSession documentSession,
            IBackChannelService backChannelService,
            IUserViewModelQuery userViewModelQuery
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(backChannelService, "backChannelService");
            Check.RequireNotNull(userViewModelQuery, "userViewModelQuery");

            _documentSession = documentSession;
            _backChannelService = backChannelService;
            _userViewModelQuery = userViewModelQuery;
        }

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<UserSession> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, domainEvent.Sender as User);
        }

        public void Handle(DomainModelUpdatedEvent<UserSession> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, domainEvent.Sender as User);
        }

        public void Execute(IDomainEvent domainEvent, UserSession userSession, User user)
        {
            // Add user to the online users channel
            _backChannelService.AddUserToOnlineUsersChannel(userSession.ConnectionId);

            // If new user session, send all online users down the wire
            if (domainEvent is DomainModelCreatedEvent<UserSession>)
            {
                // Get all user's memberships and add them to the corresponding group channel
                //var memberships = _documentSession
                //    .Query<All_Users.Result, All_Users>()
                //    .AsProjection<All_Users.Result>()
                //    .Where(x => x.UserId == user.Id)
                //    .ToList()
                //    .SelectMany(x => x.User.Memberships);

                foreach (var membership in user.Memberships)
                {
                    _backChannelService.AddUserToGroupChannel(membership.Group.Id, userSession.ConnectionId);
                }

                // Return connected users (those users active less than 5 minutes ago)
                var onlineUsers = _userViewModelQuery.BuildOnlineUserList(user.Id);

                _backChannelService.SendOnlineUsersToUserChannel(user.Id, onlineUsers);
            }

            //var userStatus = new 
            //    {
            //        User = _userViewFactory.Make(user)
            //    };

            // This is being removed to prevent asynchronous calling to clients to update user's statuses.
            // Rather than this approach, a call to update user status will respond with the current statuses
            // of all other online users.
            //_backChannelService.SendUserStatusUpdateToOnlineUsersChannel(userStatus);

            //var updateOnlineUsers = _userViewModelBuilder.BuildOnlineUserList();

            //_backChannelService.SendOnlineUsersUpdateToUserChannel(user.Id, updateOnlineUsers);
        }

        #endregion

    }
}