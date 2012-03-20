/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Threading.Tasks;
using Bowerbird.Web.Config;
using Bowerbird.Web.Services;
using Bowerbird.Web.ViewModels.Shared;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Hubs
{
    public class ActivityHub : Hub, IDisconnect
    {
        #region Members

        private readonly IHubService _hubService;

        #endregion

        #region Constructors

        public ActivityHub(
            IHubService hubService
            )
        {
            Check.RequireNotNull(hubService, "hubService");

            _hubService = hubService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void RegisterUserClient(string userId)
        {
            // persist the user client connection 
            _hubService.UpdateUserOnline(Context.ConnectionId, userId);

            // tell everyone user is online
            BroadcastUserStatusUpdate(userId);
        }

        public void BroadcastActivity(ActivityMessage message)
        {
            Check.RequireNotNull(message, "message");

            // tell all clients of activity
            Clients.activityOccurred(message);
        }

        public void BroadcastUserStatusUpdate(string userId)
        {
            // find the user
            var user = _hubService.GetUserProfile(userId);

            // tell the clients the users' status
            Clients.userStatusUpdate(user);
        }

        public Task Disconnect()
        {
            string userId;
            
            // persist session change for client
            if (_hubService.DisconnectClient(Context.ConnectionId, out userId))
            {
                // if user has no further connected sessions, tell all clients user is offline
                Clients.userStatusUpdate(_hubService.GetUserProfile(userId));
            }

            return null;
        }

        #endregion
    }
}