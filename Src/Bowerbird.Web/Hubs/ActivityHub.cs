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
    #region Models

    public class ActivityMessage
    {
        public string Type { get; set; }//observation, group, user

        public string Action { get; set; }//created,updated,deleted,joined,loggedin,commented

        public string GroupId { get; set; }

        public string WatchlistId { get; set; }

        public UserProfile User { get; set; }

        public Avatar Avatar { get; set; }

        public string Message { get; set; }//ken walker created the 'bees in carlton' project
    }

    #endregion

    #region Hubs

    [HubName("activityHub")]
    public class ActivityHub : Hub
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ActivityHub(
            IDocumentSession documentSession
           )
        {
            Check.RequireNotNull(documentSession, "documentSession");
         
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        //give the calling client (which is the browser used to connect) a GUID Id.
        //match this client browser session to the Registered user of the site
        //if the user now opens another client session, we can hook up all instances to the same user.
        //pass the Client Guid back to the browser
        public void RegisterClientUser(string userId)
        {
            var user = _documentSession.Load<User>(userId);

            var clientSessionId = new Guid(Context.ConnectionId);

            if(user != null)
            {
                var clientSessionRecord = _documentSession
                    .Load<ClientSession>()
                    .Where(x => x.ClientId == clientSessionId)
                    .FirstOrDefault();

                if (clientSessionRecord == null)
                {
                    _documentSession.Store(new ClientSession(user, clientSessionId, DateTime.Now));
                    _documentSession.SaveChanges();
                }
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
    }

    #endregion

    #region Utils

    public class ActivityGenerator
    {
        public static ActivityMessage GetNextMessage(string rootUri = "")
        {
            var messageType = new Random().Next(1, 4);

            switch (messageType)
            {
                case 1:
                    return new ActivityMessage() { Type = "user", GroupId = "projects/1", Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar.jpg" }, Message = "User did something" };
                case 2:
                    return new ActivityMessage() { Type = "group",GroupId = "projects/2",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-1.png" }, Message = "Project added" };
                case 3:
                    return new ActivityMessage() { Type = "observation",GroupId = "teams/1",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-2.png" }, Message = "Observation Created" };
                case 4:
                    return new ActivityMessage() { Type = "watchlist",GroupId = "teams/2",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar-3.png" }, Message = "Watchlist Updated" };
                default:
                    return new ActivityMessage() { Type = "user",GroupId = "organisations/1",  Avatar = new Avatar() { AltTag = "fake user", UrlToImage = rootUri + "/img/avatar.jpg" }, Message = "test message" };
            }
        }
    }

    #endregion
    
}