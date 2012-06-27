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
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Web.Factories;
using System.Collections.Generic;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Commands;

namespace Bowerbird.Web.Hubs
{
    public class ChatHub : Hub//, IDisconnect
    {
        #region Members

        private readonly IUserViewFactory _userViewFactory;
        private readonly IDocumentSession _documentSession;
        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public ChatHub(
            IUserViewFactory userViewFactory,
            IDocumentSession documentSession,
            ICommandProcessor commandProcessor)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(commandProcessor, "commandProcessor");

            _userViewFactory = userViewFactory;
            _documentSession = documentSession;
            _commandProcessor = commandProcessor;
        }

        #endregion

        #region Methods

        public void JoinChat(string chatId, string[] inviteeUserIds, string groupId)
        {
            // TODO: Only allow group members to join group chats

            // Get chat
            var chat = _documentSession.Load<Chat>(chatId);

            // Check if chat exists
            if (chat == null)
            {
                // If not, make it and add users to chat
                _commandProcessor.Process(new ChatCreateCommand()
                {
                    ChatId = chatId,
                    CreatedByUserId = Context.User.Identity.Name,
                    CreatedDateTime = DateTime.UtcNow,
                    UserIds = inviteeUserIds,
                    GroupId = groupId
                });
            }
            else
            {
                // Just add users to existing chat
                _commandProcessor.Process(new ChatUpdateCommand()
                {
                    ChatId = chatId,
                    AddUserIds = inviteeUserIds,
                    RemoveUserIds = new string[] { }
                });
            }
        }

        public void ExitChat(string chatId)
        {
            // Remove user
            _commandProcessor.Process(new ChatUpdateCommand()
            {
                ChatId = chatId,
                AddUserIds = new string[] { },
                RemoveUserIds = new[] { Context.User.Identity.Name }
            });
        }

        public void Typing(string chatId, bool isTyping)
        {
            // Notify all users of typing status
            Clients["chat-" + chatId].userIsTyping(
                new
                {
                    ChatId = chatId,
                    Timestamp = DateTime.UtcNow,
                    IsTyping = isTyping,
                    UserId = Context.User.Identity.Name
                });
        }

        public void SendChatMessage(string chatId, string messageId, string message)
        {
            // Add message to chat
            _commandProcessor.Process(new ChatMessageCreateCommand()
            {
                ChatId = chatId,
                UserId = Context.User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                MessageId = messageId,
                Message = message
            });
        }

        //public Task Disconnect()
        //{
        //    // Get user by connection id
        //    var user = GetUserByConnectionId(Context.ConnectionId);

        //    // Get chats where connection exists
        //    var chats = _documentSession
        //                    .Query<Chat>()
        //                    .Where(x => x.Users.Any(y => y.Id == user.Id))
        //                    .ToList();

        //    foreach (var chat in chats)
        //    {
        //        chat.RemoveUser(user.Id);
        //        _documentSession.Store(chat);
        //    }

        //    _documentSession.SaveChanges();

        //    return Task.Factory.StartNew(() => { });
        //}

        //private IEnumerable<User> GetUsersByConnectionIds(params string[] connectionIds)
        //{
        //    return _documentSession
        //        .Query<All_Users.Result, All_Users>()
        //        .AsProjection<All_Users.Result>()
        //        .Where(x => x.ConnectionIds.Any(y => y.In(connectionIds)))
        //        .ToList()
        //        .Select(x => x.User);
        //}

        //private User GetUserByConnectionId(string connectionId)
        //{
        //    return _documentSession
        //        .Query<All_Users.Result, All_Users>()
        //        .AsProjection<All_Users.Result>()
        //        .Where(x => x.ConnectionIds.Any(y => y == connectionId))
        //        .ToList()
        //        .Select(x => x.User)
        //        .First();
        //}

        #endregion
    }
}