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
using System.Dynamic;
using System.IO;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Log an activity item when a user joins a group
    /// </summary>
    public class ActivityUserSessionAdded : 
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

        public ActivityUserSessionAdded(
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
            Execute(domainEvent, domainEvent.DomainModel);
        }

        public void Handle(DomainModelUpdatedEvent<UserSession> domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel);
        }

        private void Execute(IDomainEvent domainEvent, UserSession userSession)
        {
            // Add user to the online users group
            _userContext.AddAuthenticatedUserSessionToOnlineUsersChannel(userSession.ConnectionId);

            // Return connected users (those users active less than 5 minutes ago)
            var onlineUsers = _userViewModelBuilder.BuildOnlineUsers();
            _userContext.GetAuthenticatedUserChannel("user-" + domainEvent.User.Id).setupOnlineUsers(onlineUsers);

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

            dynamic activity = MakeActivity(
                domainEvent,
                "userstatuschanged",
                string.Format("{0} has updated their status", domainEvent.User.GetName()),
                new[] { appRoot });

            activity.UserStatusChanged = new
            {
                User = _userViewFactory.Make(domainEvent.User)
            };

            _documentSession.Store(activity);
            _userContext.SendActivityToGroupChannel(activity);
        }

        #endregion
    }
}