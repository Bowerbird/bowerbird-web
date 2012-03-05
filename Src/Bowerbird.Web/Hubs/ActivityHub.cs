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
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Events;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
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
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public ActivityHub(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService
           )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");

            _documentSession = documentSession;
            _commandProcessor = commandProcessor;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
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
            try
            {
                User user;
                ClientSession session;

                if (UserExists(userId, out user) &&
                    !SessionExists(Context.ConnectionId, out session))
                {
                    var clientSessionCreateCommandHandler = new ClientSessionCreateCommandHandler(_documentSession);

                    clientSessionCreateCommandHandler.Handle(MakeClientSessionCreateCommand(user));
                }

                _documentSession.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ChatToUser(string userId, string message)
        {
            User user;
            List<ClientSession> openClientSessions;

            if (UserExists(Context.ConnectionId, out user) &&
                UserIsConnected(userId, out openClientSessions))
            {
                var clientMessage = new JavaScriptSerializer().Serialize(
                    new
                    {
                        Avatar = new Avatar()
                        {
                            UrlToImage = _mediaFilePathService.MakeMediaFilePath(user.Avatar.Id, "image", "thumbnail", ".jpg"),
                            AltTag = user.GetName()
                        },
                        UserName = user.GetName(),
                        CreatedDateTime = DateTime.Now,
                        Message = message
                    }
                    );

                foreach (var clientId in openClientSessions.Select(x => x.ClientId))
                {
                    Clients[clientId].messageReceived(clientMessage);
                }
            }
        }

        [Transaction]
        public void PollingRefresh()
        {
            try
            {
                ClientSession session;

                if (SessionExists(Context.ConnectionId, out session))
                {
                    var clientSessionUpdateCommandHandler = new ClientSessionUpdateCommandHandler(_documentSession);

                    clientSessionUpdateCommandHandler.Handle(MakeClientSessionUpdateCommand(session));
                }

                _documentSession.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
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

        public void GetCurrentlyConnectedUsers()
        {
            var connections = _documentSession
                .Query<ClientSessionResults, All_ClientUserSessions>()
                .AsProjection<ClientSessionResults>()
                .Where(x => x.ConnectionCreated >= DateTime.Now.AddMinutes(-11))
                .ToList();

            //identify the current connected user requesting..
            var currentUser = connections
                .Where(x => x.ClientId == Context.ConnectionId)
                .FirstOrDefault();

            // grab the connected users
            var connectedSessionUsers = connections
                .Select(x => x.UserId)
                .Distinct();

            // grab the users with their profile info
            var connectedUsers = _documentSession
                .Query<User>()
                .Where(x => x.Id.In(connectedSessionUsers) && x.Id != currentUser.UserId)
                .ToList()
                .Select(x => new
                    {
                        Avatar = new Avatar()
                            {
                                AltTag = x.GetName(),
                                UrlToImage = x.Avatar != null ? _mediaFilePathService.MakeMediaFilePath(x.Avatar.Id, "image", "thumbnail", ".jpg") : _configService.GetDefaultAvatar("user"),
                            },
                        UserName = x.GetName(),
                        UserId = x.Id
                    }
                );

            // return as list
            foreach (var user in connectedUsers)
            {
                // get all the connection ids for each user and send.
                var connectionsForUser = connections
                    .Where(x => x.UserId == user.UserId)
                    .Select(x => x.ClientId)
                    .ToList();

                foreach (var clientId in connectionsForUser)
                {
                    Clients[clientId].connectedUsers(new JavaScriptSerializer().Serialize(connectedUsers));
                }

                //Clients[clientId].connectedUsers(new JavaScriptSerializer().Serialize(connectedUsers));
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
            clientSession = _documentSession
                .Query<ClientSession>()
                .Where(x => x.ClientId == clientId)
                .FirstOrDefault();

            return clientSession != null;
        }

        private bool UserIsConnected(string userId, out List<ClientSession> clientSessions)
        {
            clientSessions = _documentSession
                .Query<ClientSession>()
                .Where(x => x.User.Id == userId && x.ConnectionCreated <= DateTime.Now.AddMinutes(-11))
                .ToList();

            return clientSessions.IsNotNullAndHasItems();
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
                    return new ActivityMessage() { Sender = "group", GroupId = "projects/2", Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-1.png" }, Message = "Project added" };
                case 3:
                    return new ActivityMessage() { Sender = "observation", GroupId = "teams/1", Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-2.png" }, Message = "Observation Created" };
                case 4:
                    return new ActivityMessage() { Sender = "watchlist", GroupId = "teams/2", Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-3.png" }, Message = "Watchlist Updated" };
                default:
                    return new ActivityMessage() { Sender = "user", GroupId = "organisations/1", Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar.jpg" }, Message = "test message" };
            }
        }
    }

    #endregion

}