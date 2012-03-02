/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using System.Linq;
using System.Threading;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Events;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Services;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client.Linq;
using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;
using System.Web.Script.Serialization;
using Raven.Client;
using SignalR.Infrastructure;

namespace Bowerbird.Web.Hubs
{
    [HubName("activityHub")]
    public class ActivityHub : Hub
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public ActivityHub(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession
           )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
         
            _documentSession = documentSession;
            _commandProcessor = commandProcessor;
        }

        #endregion

        #region Properties

        #endregion

        #region API

        //give the calling client (which is the browser used to connect) a GUID Id.
        //match this client browser session to the Registered user of the site
        //if the user now opens another client session, we can hook up all instances to the same user.
        //pass the Client Guid back to the browser
        public void RegisterClientUser(string userId)
        {
            User user;
            ClientSession session;

            if(UserExists(userId, out user) &&
                !SessionExists(Context.ConnectionId, out session))
            {
                _commandProcessor.Process(MakeClientSessionCreateCommand(user));
            }
        }

        public void PollingRefresh()
        {
            ClientSession session;

            if (SessionExists(Context.ConnectionId, out session))
            {
                _commandProcessor.Process(MakeClientSessionUpdateCommand(session));
            }
        }

        public void SendNotification(ActivityMessage message, List<string> clientIds)
        {
            if (!string.IsNullOrWhiteSpace(message.GroupId))
            {
                foreach (var clientId in clientIds)
                {
                    Clients[clientId].activityOccurred(new JavaScriptSerializer().Serialize(message));
                }
            }
        }

        #endregion

        #region Methods

        private ClientSessionCreateCommand MakeClientSessionCreateCommand(User user)
        {
            return new ClientSessionCreateCommand()
            {
                ClientId = Context.ConnectionId,
                Timestamp = DateTime.Now,
                UserId = user.Id
            };
        }

        private ClientSessionUpdateCommand MakeClientSessionUpdateCommand(ClientSession session)
        {
            return new ClientSessionUpdateCommand()
            {
                ClientId = session.ClientId,
                Timestamp = DateTime.Now,
            };
        }
       
        private bool UserExists(string userId, out User user)
        {
            user = _documentSession.Load<User>(userId);

            return user != null;
        }

        private bool SessionExists(string clientId, out ClientSession clientSession)
        {
            clientSession = _documentSession.Load<ClientSession>(clientId);

            return clientSession != null;
        }

        #endregion
    }

    #region Utils

    public class ActivityGenerator
    {
        public static ActivityMessage GetNextMessage(string rootUri = "")
        {
            var messageType = new Random().Next(1, 4);

            switch (messageType)
            {
                case 1:
                    return new ActivityMessage() { Sender = "user", GroupId = "projects/1", Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar.jpg" }, Message = "User did something" };
                case 2:
                    return new ActivityMessage() { Sender = "group",GroupId = "projects/2",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-1.png" }, Message = "Project added" };
                case 3:
                    return new ActivityMessage() { Sender = "observation",GroupId = "teams/1",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-2.png" }, Message = "Observation Created" };
                case 4:
                    return new ActivityMessage() { Sender = "watchlist",GroupId = "teams/2",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-3.png" }, Message = "Watchlist Updated" };
                default:
                    return new ActivityMessage() { Sender = "user",GroupId = "organisations/1",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar.jpg" }, Message = "test message" };
            }
        }
    }

    #endregion
    
}