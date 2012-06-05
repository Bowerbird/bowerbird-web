/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Bowerbird.Web.Hubs;
using SignalR;
using SignalR.Hubs;

namespace Bowerbird.Web.Services
{
    public class NotificationService : INotificationService
    {

        #region Fields

        private readonly IHubContext _activityHub;

        #endregion

        #region Constructors

        public NotificationService()
        {
            _activityHub = GlobalHost.ConnectionManager.GetHubContext<ActivityHub>();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SendActivity(Activity activity)
        {
            _activityHub.Clients.NewActivity(activity);
        }

        public void SendUserStatusUpdate(object userStatus)
        {
            _activityHub.Clients.UserStatusUpdate(userStatus);
        }

        #endregion
				
    }
}