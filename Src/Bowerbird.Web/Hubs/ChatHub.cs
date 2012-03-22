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
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;
using Bowerbird.Web.Services;
using Bowerbird.Web.ViewModels.Shared;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Hubs
{
    public class ChatHub : Hub
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

        #region Properties

        #endregion

        #region Methods

        public void JoinChat(string chatId)
        {
            AddToGroup(chatId);

            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, userId, Online);

            var setupChat = new SetupChat()
                                {
                                    ChatId = chatId,
                                    Title = chatId.Contains('/') ? _hubService.GetGroupName(chatId) : "Private Chat",
                                    Timestamp = DateTime.UtcNow,
                                    Users =
                                        _hubService.GetClientsForChat(chatId).Select(x => x.UserId).Distinct().Select(
                                            x => _hubService.GetUserProfile(x)).ToList(),
                                    Messages = _hubService.GetChatMessages(chatId)
                                };

            Caller.setupChat(setupChat);

            Clients[chatId].userJoinedChat(
                new UserJoinedChat()
                    {
                        ChatId = chatId,
                        Timestamp = DateTime.UtcNow,
                        User = _hubService.GetUserProfile(userId)
                    });
        }

        public void ExitChat(string chatId)
        {
            RemoveFromGroup(chatId);

            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, userId, Offline);

            if (!_hubService.GetClientsForChat(chatId).Select(x => x.UserId).Contains(userId))
            {
                Clients[chatId].userExitedChat(
                    new UserExitedChat()
                        {
                            ChatId = chatId,
                            User = _hubService.GetUserProfile(userId)
                        });
            }
        }

        public void Typing(string chatId, bool typing)
        {
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            Clients[chatId].typing(
                new ChatUserTyping()
                    {
                        ChatId = chatId,
                        Timestamp = DateTime.UtcNow,
                        Typing = typing,
                        User = _hubService.GetUserProfile(userId)
                    });
        }

        public void SendChatMessage(string chatId, string message)
        {
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            _hubService.PersistChatMessage(chatId, userId, message, null);

            Clients[chatId].chatMessageReceived(
                new ChatMessage()
                    {
                        ChatId = chatId,
                        Timestamp = DateTime.UtcNow,
                        Id = Guid.NewGuid().ToString(),
                        User = _hubService.GetUserProfile(userId),
                        //TargetUser = _hubService.GetUserProfile(targetUserId),// if directed at another user, otherwise null.
                        Message = message
                    });
        }

        #endregion
    }
}