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
            //string chatId = string.Format("{0}/{1}", type, id);

            // join the group
            AddToGroup(chatId);

            // find the user
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            // persist the chat session
            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, userId, Online);

            var setupChat = new SetupChat()
                                {
                                    ChatId = chatId,
                                    Timestamp = DateTime.UtcNow,
                                    Users =
                                        _hubService.GetClientsForChat(chatId).Select(x => x.UserId).Distinct().Select(
                                            x => _hubService.GetUserProfile(x)).ToList()
                                };

            // pass the users in the joined chat back to the new chatter
            Clients[Context.ConnectionId].setupChat(setupChat);

            // tell all the other members of the chat that a new user has joined
            Clients[chatId].chatUserStatusUpdate(
                new ChatUserStatusUpdate()
                    {
                        ChatId = chatId,
                        Status = Online,
                        Timestamp = DateTime.UtcNow,
                        User = _hubService.GetUserProfile(userId)
                    });
        }

        public void ExitChat(string chatId)
        {
            // drop the client from the chat
            RemoveFromGroup(chatId);

            // find out which user they relate to
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            // persist the offline status of the chat user
            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, userId, Offline);

            // if no other client sessions for this user exist, tell all chat members that user is offline
            if (!_hubService.GetClientsForChat(chatId).Select(x => x.UserId).Contains(userId))
            {
                Clients[chatId].chatUserStatusUpdate(
                    new ChatUserStatusUpdate()
                        {
                            ChatId = chatId,
                            Status = Offline,
                            Timestamp = DateTime.UtcNow,
                            User = _hubService.GetUserProfile(userId)
                        });
            }
        }

        public void ChatStatusUpdate(string chatId, int status)
        {
            // find which user we're updating
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            // persist state change
            _hubService.UpdateChatUserStatus(chatId, Context.ConnectionId, userId, status);

            // tell clients in the chat of users' chat status change
            Clients[chatId].chatUserStatusUpdate(
                new ChatUserStatusUpdate()
                    {
                        ChatId = chatId,
                        Status = status,
                        Timestamp = DateTime.UtcNow,
                        User = _hubService.GetUserProfile(userId)
                    });
        }

        public void Typing(string chatId, bool typing)
        {
            // find user whose typing status has changed
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            // tell all members of the group that user typing status changed
            Clients[chatId].typing(
                new ChatUserTyping()
                    {
                        ChatId = chatId,
                        Timestamp = DateTime.UtcNow,
                        Typing = typing,
                        User = _hubService.GetUserProfile(userId)
                    });
        }

        public void SendChatMessage(string chatId, string targetUserId, string message)
        {
            // find user who sent message
            var userId = _hubService.GetClientsUserId(Context.ConnectionId);

            // save the message for the chat
            _hubService.PersistChatMessage(chatId, userId, targetUserId, message);

            // send the message to the members in the chat session
            Clients[chatId].chatMessageReceived(
                new ChatMessage()
                    {
                        ChatId = chatId,
                        Timestamp = DateTime.UtcNow,
                        Id = Guid.NewGuid().ToString(),
                        User = _hubService.GetUserProfile(userId),
                        TargetUser = _hubService.GetUserProfile(targetUserId),// if directed at another user, otherwise null.
                        Message = message
                    });
        }

        #endregion
    }
}