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
using System.Linq;
using System.Threading.Tasks;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Web.Services;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Hubs
{
    public class ChatHub : Hub, IDisconnect
    {
        #region Members

        private readonly IHubService _hubService;
        private const int Online = (int)Connection.ConnectionStatus.Online;
        private const int Offline = (int)Connection.ConnectionStatus.Offline;

        #endregion

        #region Constructors

        public ChatHub(
            IHubService hubService
            )
        {
            Check.RequireNotNull(hubService, "hubService");

            _hubService = hubService;
        }

        #endregion

        #region Methods

        // Callback Methods: setupChat, userJoinedChat
        public void JoinChat(string chatId)
        {
            Check.RequireNotNullOrWhitespace(chatId, "chatId");

            var groupChatId = chatId;

            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            // Let connected users know another user has joined the chat.
            Clients[groupChatId].userJoinedChat(
                new
                {
                    ChatId = groupChatId,
                    Timestamp = DateTime.UtcNow,
                    User = _hubService.GetUserProfile(userId)
                });

            Groups.Add(Context.ConnectionId, groupChatId);

            _hubService.UpdateChatUserStatus(groupChatId, Context.ConnectionId, userId, Online);

            var setupChat = new
            {
                ChatId = groupChatId,
                Title = _hubService.GetGroupName(chatId),
                Timestamp = DateTime.UtcNow,
                Users = _hubService.GetClientsForChat(groupChatId).Select(x => x.UserId).Distinct().Select(x => _hubService.GetUserProfile(x)).ToArray(),
                Messages = _hubService.GetChatMessages(groupChatId)
            };

            Caller.setupChat(setupChat);
        }

        // Callback Methods: chatRequest
        public void StartChat(string chatId, string userId)
        {
            Check.RequireNotNullOrWhitespace(chatId, "chatId");
            Check.RequireNotNullOrWhitespace(userId, "userId");

            var privateChatId = chatId;

            Caller.debugToLog(string.Format("ChatHub.startChat - chatId:{0} userId:{1}", privateChatId, userId));

            Groups.Add(Context.ConnectionId, privateChatId);

            var chatUserId = _hubService.GetClientsUserId(Context.ConnectionId);

            var fromUser = _hubService.GetUserProfile(chatUserId);

            var toUser = _hubService.GetUserProfile(userId);

            _hubService.UpdateChatUserStatus(privateChatId, Context.ConnectionId, chatUserId, Online);

            var clientIds = _hubService.GetConnectedClientIdsForAUser(userId);

            var comeToChat = string.Format("{0} has invited you to chat", fromUser.Name);

            var chatRequest = new
            {
                ChatId = privateChatId,
                Title = string.Format("{0}...", fromUser.Name),
                Messages = new[] { new { ChatId = privateChatId, Message = comeToChat, Timestamp = DateTime.UtcNow } },
                Users = new[] {fromUser, toUser},
                Timestamp = DateTime.UtcNow
            };

            foreach (var clientId in clientIds)
            {
                Groups.Add(clientId.ToString(), privateChatId);
            }

            Clients[privateChatId].chatRequest(chatRequest);
        }

        // Callback Methods: userExitedChat
        public void ExitChat(string chatId)
        {
            Groups.Remove(Context.ConnectionId, chatId);

            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, userId, Offline);

            if (!_hubService.GetClientsForChat(chatId).Select(x => x.UserId).Contains(userId))
            {
                Clients[chatId].userExitedChat(
                    new
                    {
                        ChatId = chatId,
                        User = _hubService.GetUserProfile(userId)
                    });
            }
        }

        // Callback Methods: typing
        public void Typing(string chatId, bool typing)
        {
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            Clients[chatId].typing(
                new
                {
                    ChatId = chatId,
                    Timestamp = DateTime.UtcNow,
                    Typing = typing,
                    User = _hubService.GetUserProfile(userId)
                });
        }

        // Callback Methods: chatMessageReceived
        public void SendChatMessage(string chatId, string message)
        {
            //var groupChatId = "chat-" + chatId;

            Caller.debugToLog(string.Format("ChatHub.sendChatMessage - chatId:{0} message:{1}", chatId, message));

            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            _hubService.PersistChatMessage(chatId, userId, message, null);

            Clients[chatId].chatMessageReceived(
                new
                {
                    ChatId = chatId,
                    Timestamp = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    User = _hubService.GetUserProfile(userId),
                    Message = message
                });
        }

        public Task Disconnect()
        {
            return Clients.userStatusUpdate(_hubService.GetUserProfile(_hubService.DisconnectClient(Context.ConnectionId)));
        }

        #endregion
    }
}