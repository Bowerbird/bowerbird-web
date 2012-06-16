///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Threading.Tasks;
//using Bowerbird.Web.Services;
//using SignalR.Hubs;
//using Bowerbird.Core.DesignByContract;

//namespace Bowerbird.Web.Hubs
//{
//    public class NotificationHub : Hub, IDisconnect
//    {
//        #region Members

//        private readonly IHubService _hubService;

//        #endregion

//        #region Constructors

//        public NotificationHub(
//            IHubService hubService
//            )
//        {
//            Check.RequireNotNull(hubService, "hubService");

//            _hubService = hubService;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public void RegisterUserClient(string userId)
//        {
//            _hubService.UpdateUserOnline(Context.ConnectionId, userId);

//            BroadcastUserStatusUpdate(userId);
//        }

//        public void BroadcastUserStatusUpdate(string userId)
//        {
//            Clients.userStatusUpdate(_hubService.GetUserProfile(userId));
//        }

//        public Task Disconnect()
//        {
//            return Clients.userStatusUpdate(_hubService.GetUserProfile(_hubService.DisconnectClient(Context.ConnectionId)));
//        }

//        #endregion
//    }
//}