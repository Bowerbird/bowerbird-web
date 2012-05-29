///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System;
//using System.Collections.Generic;
//using Bowerbird.Core.Commands;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.DomainModels.Sessions;
//using Bowerbird.Web.Hubs;
//using Raven.Client;
//using Raven.Client.Linq;
//using SignalR;
//using SignalR.Hosting.AspNet;
//using SignalR.Infrastructure;
//using System.Linq;

//namespace Bowerbird.Web.Notifications
//{
//    public class NotificationProcessor : INotificationProcessor
//    {
//        #region Members

//        private readonly IDocumentSession _documentSession;
//        private readonly ICommandProcessor _commandProcessor;

//        #endregion

//        #region Constructors

//        public NotificationProcessor(
//            IDocumentSession documentSession,
//            ICommandProcessor commandProcessor)
//        {
//            Check.RequireNotNull(documentSession, "documentSession");
//            Check.RequireNotNull(commandProcessor, "commandProcessor");

//            _documentSession = documentSession;
//            _commandProcessor = commandProcessor;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public void Notify<T>(T model, IEnumerable<string> groups, Action<dynamic, T> callClient)
//        {
//            Check.RequireNotNull(model, "model");
//            Check.RequireNotNull(callClient, "callClient");

//            // Get the user client ids for the members of the given groups, who are currently logged in
//            var userIds = _documentSession
//                .Query<Member>()
//                .Where(x => x.Group.Id.In(groups))
//                .ToList()
//                .Select(x => x.User.Id);

//            var connectedIds = _documentSession
//                .Query<UserSession>()
//                .Where(x => x.User.Id.In(userIds))
//                .ToList()
//                .Select(x => x.ClientId);

//            // Call each client id with notification
//            var clients = AspNetHost.DependencyResolver.Resolve<IConnectionManager>().GetClients<NotificationHub>();

//            foreach (var clientId in connectedIds)
//            {
//                callClient(clients[clientId], model);
//            }
//        }

//        #endregion
//    }
//}