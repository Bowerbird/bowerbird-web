/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.Config;
using SignalR;
using Bowerbird.Web.Hubs;
using SignalR.Hubs;

namespace Bowerbird.Web.Services
{
    public class BackChannelService : IBackChannelService
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IConnectionManager _connectionManager;

        private static readonly object _userHubLock = new object();
        private static readonly object _groupHubLock = new object();
        private static readonly object _chatHubLock = new object();
        private static readonly object _debugHubLock = new object();

        #endregion

        #region Constructors

        public BackChannelService(
            IDocumentSession documentSession,
            IConnectionManager connectionManager
           )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(connectionManager, "connectionManager");

            _documentSession = documentSession;
            _connectionManager = connectionManager;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #region User Channel

        public void AddUserToUserChannel(string userId, string connectionId)
        {
            if (ChannelServiceOff()) return;

            lock (_userHubLock)
            {
                GetHub<UserHub>().Groups.Add(connectionId, "user-" + userId);
            }
        }

        public void SendJoinedGroupToUserChannel(string userId, object group)
        {
            if (ChannelServiceOff()) return;

            lock (_userHubLock)
            {
                GetHub<UserHub>().Clients["user-" + userId].joinedGroup(group);
            }
        }

        /// <summary>
        /// Rather than call all clients when one user updates their status, 
        /// call the client that updated and pass the status of all the users.
        /// </summary>
        public void SendOnlineUsersUpdateToUserChannel(string userId, object onlineUsers)
        {
            if (ChannelServiceOff()) return;

            GetHub<UserHub>().Clients["user-" + userId].updateOnlineUsers(onlineUsers);
        }

        public void SendOnlineUsersToUserChannel(string userId, object onlineUsers)
        {
            if (ChannelServiceOff()) return;

            lock (_userHubLock)
            {
                GetHub<UserHub>().Clients["user-" + userId].setupOnlineUsers(onlineUsers);
            }
        }

        public void NotifyChatJoinedToUserChannel(string userId, object chatDetails)
        {
            if (ChannelServiceOff()) return;

            lock (_userHubLock)
            {
                GetHub<UserHub>().Clients["user-" + userId].chatJoined(chatDetails);
            }
        }

        public void NotifyChatExitedToUserChannel(string userId, string chatId)
        {
            if (ChannelServiceOff()) return;

            lock (_userHubLock)
            {
                GetHub<UserHub>().Clients["user-" + userId].chatExited(chatId);
            }
        }

        public void NotifyMediaResourceUploadSuccessToUserChannel(string userId, object mediaResource)
        {
            if (ChannelServiceOff()) return;

            GetHub<UserHub>().Clients["user-" + userId].mediaResourceUploadSuccess(mediaResource);
        }

        public void NotifyMediaResourceUploadFailureToUserChannel(string userId, string key, string reason)
        {
            if (ChannelServiceOff()) return;

            GetHub<UserHub>().Clients["user-" + userId].mediaResourceUploadFailure(key, reason);
        }

        #endregion

        #region Group Channel

        public void AddUserToGroupChannel(string groupId, string connectionId)
        {
            if (ChannelServiceOff()) return;

            lock (_groupHubLock)
            {
                GetHub<GroupHub>().Groups.Add(connectionId, "group-" + groupId);
            }
        }

        public void SendActivityToGroupChannel(dynamic activity)
        {
            if (ChannelServiceOff()) return;

            lock (_groupHubLock)
            {
                var groupHub = GetHub<GroupHub>();

                foreach (var group in activity.Groups)
                {
                    groupHub.Clients["group-" + group.Id].newActivity(group.Id, activity);
                }
            }
        }

        #endregion

        #region Online Users Channel

        public void AddUserToOnlineUsersChannel(string connectionId)
        {
            if (ChannelServiceOff()) return;

            lock (_userHubLock)
            {
                GetHub<UserHub>().Groups.Add(connectionId, "online-users");
            }
        }

        public void SendUserStatusUpdateToOnlineUsersChannel(object userStatus)
        {
            if (ChannelServiceOff()) return;

            lock (_userHubLock)
            {
                GetHub<UserHub>().Clients["online-users"].userStatusUpdate(userStatus);
            }
        }

        #endregion

        #region Chat Channel

        public void AddUserToChatChannel(string chatId, string connectionId)
        {
            if (ChannelServiceOff()) return;

            lock (_chatHubLock)
            {
                GetHub<ChatHub>().Groups.Add(connectionId, "chat-" + chatId);
            }
        }

        public void RemoveUserFromChatChannel(string chatId, string connectionId)
        {
            if (ChannelServiceOff()) return;

            lock (_chatHubLock)
            {
                GetHub<ChatHub>().Groups.Remove(connectionId, "chat-" + chatId);
            }
        }

        public void UserJoinedChatToChatChannel(string chatId, object chatMessageDetails)
        {
            if (ChannelServiceOff()) return;

            lock (_chatHubLock)
            {
                GetHub<ChatHub>().Clients["chat-" + chatId].userJoinedChat(chatMessageDetails);
            }
        }

        public void UserExitedChatToChatChannel(string chatId, object chatMessageDetails)
        {
            if (ChannelServiceOff()) return;

            lock (_chatHubLock)
            {
                GetHub<ChatHub>().Clients["chat-" + chatId].userExitedChat(chatMessageDetails);
            }
        }

        public void NewChatMessageToChatChannel(string chatId, object chatMessageDetails)
        {
            if (ChannelServiceOff()) return;

            lock (_chatHubLock)
            {
                GetHub<ChatHub>().Clients["chat-" + chatId].newChatMessage(chatMessageDetails);
            }
        }

        #endregion

        #region Debug Channel

        public void DebugToClient(object output)
        {
            if (ChannelServiceOff()) return;

            lock (_debugHubLock)
            {
                _connectionManager.GetHubContext<DebugHub>().Clients.debugToClient(output);
            }
        }

        #endregion

        private IHubContext GetHub<T>() where T : IHub
        {
            return _connectionManager.GetHubContext<T>();
        }

        private bool ChannelServiceOff()
        {
            return !_documentSession.Load<AppRoot>(Constants.AppRootId).BackChannelServiceStatus;
        }

        #endregion
    }
}