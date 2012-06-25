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
using System.Dynamic;
using System.IO;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.Config;
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
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public UserSessionUpdated(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IUserViewModelBuilder userViewModelBuilder,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _userViewModelBuilder = userViewModelBuilder;
            _userContext = userContext;
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
            _userContext.AddUserToOnlineUsersChannel(userSession.ConnectionId);

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
                    _userContext.AddUserToGroupChannel(membership.Group.Id, userSession.ConnectionId);
                }

                // Return connected users (those users active less than 5 minutes ago)
                var onlineUsers = _userViewModelBuilder.BuildOnlineUsers();
                _userContext.GetUserChannel(user.Id).setupOnlineUsers(onlineUsers);
            }

            var userStatus = new 
                {
                    User = _userViewFactory.Make(user),
                    LatestActivity = userSession.LatestActivity
                };

            _userContext.GetOnlinerUsersChannel().userStatusUpdate(userStatus);
        }

        #endregion

    }
}