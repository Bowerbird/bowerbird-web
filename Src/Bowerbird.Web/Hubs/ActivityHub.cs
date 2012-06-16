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
using Bowerbird.Web.Services;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;
using Raven.Client;

namespace Bowerbird.Web.Hubs
{
    public class ActivityHub : Hub//, IDisconnect
    {
        #region Members

        #endregion

        #region Constructors

        public ActivityHub() {}

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

        //public void RegisterUserClient(string userId)
        //{
        //    _hubService.UpdateUserOnline(Context.ConnectionId, userId);

        //    BroadcastUserStatusUpdate(userId);
        //}

        //public void BroadcastUserStatusUpdate(string userId)
        //{
        //    Clients.userStatusUpdate(_hubService.GetUserProfile(userId));
        //}

        //public Task Disconnect()
        //{
        //    return Clients.userStatusUpdate(_hubService.GetUserProfile(_hubService.DisconnectClient(Context.ConnectionId)));
        //}

        #endregion

    }
}