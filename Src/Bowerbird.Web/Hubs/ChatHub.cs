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
            Groups.Add(Context.ConnectionId, chatId);

            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, userId, Online);

            var setupChat = new
            {
                ChatId = chatId,
                Title = _hubService.GetGroupName(chatId),
                Timestamp = DateTime.UtcNow,
                Users =
                    _hubService.GetClientsForChat(chatId).Select(x => x.UserId).Distinct().Select(
                        x => _hubService.GetUserProfile(x)).ToList(),
                Messages = _hubService.GetChatMessages(chatId)
            };

            Caller.setupChat(setupChat);

            Clients[chatId].userJoinedChat(
                new
                {
                    ChatId = chatId,
                    Timestamp = DateTime.UtcNow,
                    User = _hubService.GetUserProfile(userId)
                });
        }

        // Callback Methods: chatRequest
        public void StartChat(string chatId, string userId)
        {
            Groups.Add(Context.ConnectionId, chatId);

            var chatUserId = _hubService.GetClientsUserId(Context.ConnectionId);

            var fromUser = _hubService.GetUserProfile(chatUserId);

            var toUser = _hubService.GetUserProfile(userId);

            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, chatUserId, Online);

            var clientIds = _hubService.GetConnectedClientIdsForAUser(userId);

            var comeToChat = string.Format("{0} has invited you to chat", fromUser.Name);

            var chatRequest = new
            {
                ChatId = chatId,
                FromUser = fromUser,
                ToUser = toUser,
                Id = Guid.NewGuid().ToString(),
                Message = comeToChat,
                Timestamp = DateTime.UtcNow
            };

            foreach (var clientId in clientIds)
            {
                Clients[clientId].chatRequest(chatRequest);
            }
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