using System;
using Bowerbird.Core.Config;
using Bowerbird.Core.Services;
using Bowerbird.Core.ViewModelFactories;
using Microsoft.AspNet.SignalR;
using Bowerbird.Core.DesignByContract;
using Raven.Client;
using System.Linq;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Hubs
{
    public class UserHub : Hub
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IOnlineUserCache _onlineUserCache;

        #endregion

        #region Constructors

        public UserHub(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IBackChannelService backChannelService,
            IOnlineUserCache onlineUserCache)

        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(backChannelService, "backChannelService");
            Check.RequireNotNull(onlineUserCache, "onlineUserCache");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _backChannelService = backChannelService;
            _onlineUserCache = onlineUserCache;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public dynamic RegisterUserClient(string userId)
        {
            var user = _documentSession.Load<User>(userId);
            
            // Add user to their own group
            Groups.Add(Context.ConnectionId, "user-" + userId);

            // Get all user's memberships and add them to the corresponding group channel
            foreach (var membership in user.Memberships)
            {
                _backChannelService.AddUserToGroupChannel(membership.Group.Id, Context.ConnectionId);
            }

            _backChannelService.AddUserToOnlineUsersChannel(Context.ConnectionId);

            // ADD TO CACHE
            _onlineUserCache.AddUserSession(userId, Context.ConnectionId);

            return BuildOnlineUserList();
        }

        /// <summary>
        /// Passing heartbeat and interactivity from the client to keep the time structure independent of the server
        /// </summary>
        public dynamic UpdateUserClientStatus(string userId, DateTime latestHeartbeat, DateTime latestInteractivity)
        {
            _onlineUserCache.UpdateUserSession(userId, Context.ConnectionId, latestHeartbeat, latestInteractivity);

            return BuildOnlineUserList();
        }

        private object BuildOnlineUserList()
        {
            // Return connected users (those users active less than 5 minutes ago)
            var fiveMinutesAgo = DateTime.UtcNow - TimeSpan.FromMinutes(5);

            return _onlineUserCache
                .CurrentlyOnlineUsers(fiveMinutesAgo)
                .Select(x => _userViewFactory.Make(x.User, null));
        }

        #endregion

    }
}