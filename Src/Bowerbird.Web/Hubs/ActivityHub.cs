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
using System.Threading.Tasks;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.Services;
using Raven.Client.Linq;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;
using Raven.Client;

namespace Bowerbird.Web.Hubs
{
    public class ActivityHub : Hub, IDisconnect
    {
        #region Members

        private readonly IHubService _hubService;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ActivityHub(
            IHubService hubService,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(hubService, "hubService");
            Check.RequireNotNull(documentSession, "documentSession");

            _hubService = hubService;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void JoinGroupStreams(string userId, string[] groupIds)
        {
            foreach (var groupId in groupIds)
            {
                Groups.Add(Context.ConnectionId, "stream-" + groupId);
            }
        }

        public void LeaveGroupStreams(string userId, string[] groupIds)
        {
            foreach (var groupId in groupIds)
            {
                Groups.Remove(Context.ConnectionId, "stream-" + groupId);
            }
        }

        public void RegisterUserClient(string userId)
        {
            var groupIds = _documentSession.Query<All_Groups.Result, All_Groups>()
                    .Where(x => x.UserIds.Any(y => y == userId))
                    .AsProjection<All_Groups.Result>()
                    .ToList()
                    .Select(x => x.GroupId);

            foreach (var groupId in groupIds)
            {
                Groups.Add(Context.ConnectionId, "stream-" + groupId);
            }

            _hubService.UpdateUserOnline(Context.ConnectionId, userId);

            Clients.userStatusUpdate(_hubService.GetUserProfile(userId));

            var connectedUserIds = _documentSession
                .Query<All_Sessions.Results, All_Sessions>()
                .AsProjection<All_Sessions.Results>()
                .Include(x => x.UserId)
                .Where(x => x.Status < (int) Connection.ConnectionStatus.Offline && x.LatestActivity > DateTime.UtcNow.AddHours(-1))
                .ToList()
                .Select(x => x.UserId)
                .Distinct();

            var connectedUsers = _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.UserId.In(connectedUserIds))
                .AsProjection<All_Users.Result>()
                .ToList()
                .Select(x => MakeUser(x.User));

            Caller.setupOnlineUsers(connectedUsers);
        }

        public Task Disconnect()
        {
            return Clients.userStatusUpdate(_hubService.GetUserProfile(_hubService.DisconnectClient(Context.ConnectionId)));
        }

        private object MakeUser(User user)
        {
            return new
            {
                user.Id,
                Avatar = user.Avatar,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        #endregion

    }
}