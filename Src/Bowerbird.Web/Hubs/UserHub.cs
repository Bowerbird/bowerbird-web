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
using System.Dynamic;
using System.Threading.Tasks;
using Bowerbird.Core.Services;
using Microsoft.AspNet.SignalR;
using Bowerbird.Core.DesignByContract;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Queries;

namespace Bowerbird.Web.Hubs
{
    public class UserHub : Hub
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewModelQuery _userViewModelQuery;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public UserHub(
            IDocumentSession documentSession,
            IUserViewModelQuery userViewModelQuery,
            IBackChannelService backChannelService)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewModelQuery, "userViewModelQuery");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _userViewModelQuery = userViewModelQuery;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void RegisterUserClient(string userId)
        {
            var user = _documentSession.Load<User>(userId);

            // Add user to their own group
            Groups.Add(Context.ConnectionId, "user-" + userId);

            // Update user's status to online
            user.AddSession(Context.ConnectionId);
            _documentSession.Store(user);
            _documentSession.SaveChanges();

            // Return all online uses to newly connected client
            //return _userViewModelQuery.BuildOnlineUserList();
        }

        /// <summary>
        /// Passing heartbeat and interactivity from the client to keep the time structure independent of the server
        /// </summary>
        public dynamic UpdateUserClientStatus(string userId, DateTime latestHeartbeat, DateTime latestInteractivity)
        {
            var user = GetUserByConnectionId(Context.ConnectionId);

            //if (user != null)
            //{
                user.UpdateSessionLatestActivity(Context.ConnectionId, latestHeartbeat, latestInteractivity);

                _documentSession.Store(user);
                _documentSession.SaveChanges();
            //}

            //dynamic response = new ExpandoObject();

            return _userViewModelQuery.BuildOnlineUserList(user.Id);

            //response.onlineUsers = _userViewModelQuery.BuildOnlineUserList();
/*
#if !JS_COMBINE_MINIFY
            _backChannelService.DebugToClient("SERVER onlineUsers:");
            _backChannelService.DebugToClient(onlineUsers);
#endif  
*/
            //return response;

            //return onlineUsers;
        }

        //public Task Disconnect()
        //{
        //    // Remove this connection session from user
        //    var user = GetUserByConnectionId(Context.ConnectionId);
        //    // TODO: This throws an exception when user not found.. 
        //    user.RemoveSession(Context.ConnectionId);
        //    _documentSession.Store(user);
        //    _documentSession.SaveChanges();

        //    Groups.Remove(Context.ConnectionId, "online-users");
        //    Groups.Remove(Context.ConnectionId, "user-" + user.Id);

        //    return Task.Factory.StartNew(() => { });
        //}

        private User GetUserByConnectionId(string connectionId)
        {
            var result = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.ConnectionIds.Any(y => y == connectionId))
                .ToList()
                .FirstOrDefault();

            return result != null ? result.User : null;
        }

        #endregion

    }
}