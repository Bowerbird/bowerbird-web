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
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public ChatHub(
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService
           )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");

            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region API

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

        public void SendChatMessage(string userId, string groupId, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                foreach (var clientId in GetCurrentlyConnectedUserClients())
                {
                    Clients[clientId].chatMessageReceived(
                        new ChatMessage()
                            {
                                Id = Guid.NewGuid().ToString(),
                                GroupId = groupId,
                                Message = message,
                                Timestamp = DateTime.Now,
                                User = new UserProfile()
                                           {
                                               Id = userId
                                           }
                            }
                    );
                }
            }
        }

        public void GetConnectedChatUsers(string groupId)
        {
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                var connectedUsers = GetCurrentlyConnectedUsers(groupId);

                Clients[Context.ConnectionId].connectedUsers(new {connectedUsers, groupId});
            }
        }

        public List<UserProfile> GetCurrentlyConnectedUsers(string groupId)
        {
            var connectedSessionUsers = _documentSession
                .Query<ClientSessionResults, All_ClientUserSessions>()
                .AsProjection<ClientSessionResults>()
                .Where(x => x.ConnectionCreated >= DateTime.Now.AddMinutes(-11))
                .ToList();

            var connectedUserIds = connectedSessionUsers
                .Select(x => x.UserId)
                .Distinct();

            var groupUsers = _documentSession.Query<GroupMember>()
                .Where(x => x.Group.Id == groupId && x.User.Id.In(connectedUserIds))
                .Include(x => x.User.Id)
                .ToList();

            // all the system users who are in this group and currently connected..
            var connectedUsers = _documentSession
                .Query<User>()
                .Where(x => x.Id.In(groupUsers.Select(y => y.User.Id)))
                .ToList()
                .Select(x => new UserProfile()
                    {
                        Avatar = new Avatar()
                                    {
                                        AltTag = x.GetName(),
                                        UrlToImage =
                                            x.Avatar != null
                                                ? _mediaFilePathService.MakeMediaFilePath(x.Avatar.Id,
                                                                                        "image",
                                                                                        "thumbnail",
                                                                                        ".jpg")
                                                : _configService.GetDefaultAvatar("user"),
                                    },
                        Name = x.GetName(),
                        Id = x.Id
                    });

            return connectedUsers.ToList();

        }

        //public void GetCurrentlyConnectedUsers()
        //{
        //    var connections = _documentSession
        //        .Query<ClientSessionResults, All_ClientUserSessions>()
        //        .AsProjection<ClientSessionResults>()
        //        .Where(x => x.ConnectionCreated >= DateTime.Now.AddMinutes(-11))
        //        .ToList();

        //    //identify the current connected user requesting..
        //    var currentUser = connections
        //        .Where(x => x.ClientId == Context.ConnectionId)
        //        .FirstOrDefault();

        //    // grab the connected users
        //    var connectedSessionUsers = connections
        //        .Select(x => x.UserId)
        //        .Distinct();

        //    // grab the users with their profile info
        //    var connectedUsers = _documentSession
        //        .Query<User>()
        //        .Where(x => x.Id.In(connectedSessionUsers) && x.Id != currentUser.UserId)
        //        .ToList()
        //        .Select(x => new
        //            {
        //                Avatar = new Avatar()
        //                    {
        //                        AltTag = x.GetName(),
        //                        UrlToImage = x.Avatar != null ? _mediaFilePathService.MakeMediaFilePath(x.Avatar.Id, "image", "thumbnail", ".jpg") : _configService.GetDefaultAvatar("user"),
        //                    },
        //                UserName = x.GetName(),
        //                UserId = x.Id
        //            }
        //        );

        //    // return as list
        //    foreach (var user in connectedUsers)
        //    {
        //        // get all the connection ids for each user and send.
        //        var connectionsForUser = connections
        //            .Where(x => x.UserId == user.UserId)
        //            .Select(x => x.ClientId)
        //            .ToList();

        //        foreach (var clientId in connectionsForUser)
        //        {
        //            Clients[clientId].connectedUsers(new JavaScriptSerializer().Serialize(connectedUsers));
        //        }
        //    }
        //}

        #endregion

        #region Methods

        private List<string> GetCurrentlyConnectedUserClients()
        {
            return _documentSession
                .Query<ClientSessionResults, All_ClientUserSessions>()
                .AsProjection<ClientSessionResults>()
                .Where(x => x.ConnectionCreated >= DateTime.Now.AddMinutes(-11))
                .ToList()
                .Select(x => x.ClientId)
                .ToList();
        }

        private bool SessionExists(string clientId, out ClientSession clientSession)
        {
            clientSession = _documentSession
                .Query<ClientSession>()
                .Where(x => x.ClientId == clientId)
                .FirstOrDefault();

            return clientSession != null;
        }

        private ClientSessionUpdateCommand MakeClientSessionUpdateCommand(ClientSession session)
        {
            return new ClientSessionUpdateCommand()
            {
                ClientId = session.ClientId,
                Timestamp = DateTime.Now,
            };
        }

        #endregion
    }

    public class ChatMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("user")]
        public UserProfile User { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}