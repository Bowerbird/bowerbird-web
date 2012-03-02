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
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Hubs;
using Raven.Client;
using Raven.Client.Linq;
using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;
using System.Linq;

namespace Bowerbird.Web.Notifications
{
    public class NotificationProcessor : INotificationProcessor
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public NotificationProcessor(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Notify(Activity activity, IEnumerable<string> userIds)
        {
            Check.RequireNotNull(activity, "activity");
            
            // Create notification
            var notification = new Notification(
                activity, 
                DateTime.Now, 
                userIds);

            // Persist notification
            _documentSession.Store(notification);

            // load clientIds from raven using userIds..
            var connectedIds = _documentSession
                .Query<ClientSession>()
                .Where(x => x.User.Id.In(userIds))
                .Select(x => x.ClientId.ToString())
                .ToList();

            // Call Hub
            var clients = AspNetHost.DependencyResolver.Resolve<IConnectionManager>().GetClients<ActivityHub>();

            foreach (var clientId in connectedIds)
            {
                clients[clientId].activityOccurred(new JavaScriptSerializer().Serialize(activity.Message));
            }

        }

        #endregion
    }
}