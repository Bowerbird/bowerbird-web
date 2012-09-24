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
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.EventHandlers
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
        private readonly IUserViewFactory _userViewFactory;
        private readonly IBackChannelService _backChannelService;
        private readonly IUserViewModelBuilder _userViewModelBuilder;

        #endregion

        #region Constructors

        public UserSessionUpdated(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IBackChannelService backChannelService,
            IUserViewModelBuilder userViewModelBuilder
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(backChannelService, "backChannelService");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _backChannelService = backChannelService;
            _userViewModelBuilder = userViewModelBuilder;
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
                var memberships = _documentSession
                    .Query<All_Users.Result, All_Users>()
                    .AsProjection<All_Users.Result>()
                    .Where(x => x.UserId == user.Id)
                    .ToList()
                    .SelectMany(x => x.User.Memberships);

                foreach (var membership in memberships)
                {
                    _backChannelService.AddUserToGroupChannel(membership.Group.Id, userSession.ConnectionId);
                }

                // Return connected users (those users active less than 5 minutes ago)
                var onlineUsers = _userViewModelBuilder.BuildOnlineUserList();

                _backChannelService.SendOnlineUsersToUserChannel(user.Id, onlineUsers);
            }

            var userStatus = new 
                {
                    User = _userViewFactory.Make(user)
                };

            _backChannelService.SendUserStatusUpdateToOnlineUsersChannel(userStatus);
        }

        #endregion

    }
}