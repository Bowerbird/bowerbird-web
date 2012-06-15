/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.Services;
using Bowerbird.Web.Hubs;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Services
{
    public class NotificationService : INotificationService
    {

        #region Fields

        private readonly IHubContext _hubContext;

        #endregion

        #region Constructors
        public NotificationService(
            [HubContext(typeof(ActivityHub))] IHubContext hubContext)
        {
            Check.RequireNotNull(hubContext, "hubContext");

            _hubContext = hubContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SendActivity(dynamic activity)
        {
            foreach(var group in activity.Groups)
            {
                _hubContext.Clients["stream-" + group.Id].newActivity(activity);
            }
        }

        public void SendUserStatusUpdate(object userStatus)
        {
            _hubContext.Clients.userStatusUpdate(userStatus);
        }

        #endregion
				
    }
}