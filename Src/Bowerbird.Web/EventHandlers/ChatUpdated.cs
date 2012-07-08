/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Raven.Client.Linq;
using SignalR.Hubs;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.Config;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.Services;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Sends a chat message to clients. The situations in which this can occur are:
    /// - A new chat is created
    /// - A chat message is added to an existing chat
    /// </summary>
    public class ChatUpdated : 
        DomainEventHandlerBase, 
        IEventHandler<DomainModelCreatedEvent<Chat>>, 
        IEventHandler<DomainModelCreatedEvent<ChatMessage>>,
        IEventHandler<UserJoinedChatEvent>,
        IEventHandler<UserExitedChatEvent>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public ChatUpdated(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Chat> domainEvent)
        {
            System.Diagnostics.Debug.WriteLine("Async Document Session: {0} HasChanges: {1}, NumberOfRequests: {2}.", ((Raven.Client.Document.DocumentSession)_documentSession).Id, _documentSession.Advanced.HasChanges, _documentSession.Advanced.NumberOfRequests);

            // Get chat
            var chat = domainEvent.DomainModel;

            // Add all users to chat channel
            var users = _documentSession.Load<User>(chat.Users.Select(x => x.Id)).ToList();
            foreach (var connectionId in users.SelectMany(x => x.Sessions.Select(y => y.ConnectionId)))
            {
                _backChannelService.AddUserToChatChannel(chat.Id, connectionId);
            }

            // Notify initiator they have joined chat
            NotifyUserTheyHaveJoinedChat(chat, domainEvent.User.Id);
        }

        public void Handle(UserJoinedChatEvent domainEvent)
        {
            // Get chat
            var chat = domainEvent.Chat;

            // Add user to chat channel
            foreach (var session in domainEvent.User.Sessions)
            {
                _backChannelService.AddUserToChatChannel(chat.Id, session.ConnectionId);
            }

            // Notify user they have joined chat
            NotifyUserTheyHaveJoinedChat(chat, domainEvent.User.Id);

            // Notify all users in chat that new user has joined
            var chatMessageDetails = new
            {
                Id = Guid.NewGuid().ToString(),
                Type = "useradded",
                ChatId = chat.Id,
                Timestamp = DateTime.UtcNow,
                Message = string.Format("{0} has joined the chat", domainEvent.User.GetName()),
                FromUser = _userViewFactory.Make(domainEvent.User)
            };

            _backChannelService.UserJoinedChatToChatChannel(chat.Id, chatMessageDetails);
        }

        public void Handle(UserExitedChatEvent domainEvent)
        {
            // Get chat
            var chat = domainEvent.Chat;

            // Remove user from chat channel
            foreach (var session in domainEvent.User.Sessions)
            {
                _backChannelService.RemoveUserFromChatChannel(chat.Id, session.ConnectionId);
            }

            // Notify all of leaving user's sessions that they have left
            _backChannelService.NotifyChatExitedToUserChannel(domainEvent.User.Id, chat.Id);

            // Notify all users in chat that user has left chat
            var chatMessageDetails = new
            {
                Id = Guid.NewGuid().ToString(),
                Type = "useradded",
                ChatId = chat.Id,
                Timestamp = DateTime.UtcNow,
                Message = string.Format("{0} has left the chat", domainEvent.User.GetName()),
                FromUser = _userViewFactory.Make(domainEvent.User)
            };

            _backChannelService.UserExitedChatToChatChannel(chat.Id, chatMessageDetails);
        }

        public void Handle(DomainModelCreatedEvent<ChatMessage> domainEvent)
        {
            // Get chat
            var chat = domainEvent.Sender as Chat;

            // Get message
            var chatMessage = domainEvent.DomainModel;

            // Get sender of message
            var messageSender = _documentSession.Load<User>(chatMessage.User.Id);

            // Get all users in chat
            var users = _documentSession.Load<User>(chat.Users.Select(x => x.Id)).ToList();

            // Add the sender in the case that they are re-joining a chat
            if (users.All(x => x.Id != domainEvent.User.Id))
            {
                users.Add(domainEvent.User);
            }

            // Ensure all users' clients are still subscribed to chat
            foreach (var connectionId in users.SelectMany(x => x.Sessions.Select(y => y.ConnectionId)))
            {
                _backChannelService.AddUserToChatChannel(chat.Id, connectionId);
            }

            // Send message to clients
            dynamic chatMessageDetails = new ExpandoObject();

            chatMessageDetails.Id = chatMessage.Id;
            chatMessageDetails.Type = "usermessage";
            chatMessageDetails.ChatId = chat.Id;
            chatMessageDetails.Timestamp = chatMessage.Timestamp;
            chatMessageDetails.Message = chatMessage.Message;
            chatMessageDetails.FromUser = _userViewFactory.Make(messageSender);

            _backChannelService.NewChatMessageToChatChannel(chat.Id, chatMessageDetails);
        }

        private void NotifyUserTheyHaveJoinedChat(Chat chat, string userId)
        {
            var users = _documentSession.Load<User>(chat.Users.Select(x => x.Id));
            var chatMessageUsers = _documentSession.Load<User>(chat.Messages.Select(x => x.User.Id).Distinct());

            dynamic chatDetails = new ExpandoObject();

            chatDetails.ChatId = chat.Id;
            chatDetails.Users = users.Select(_userViewFactory.Make);

            if (chat.ChatType == "group")
            {
                chatDetails.Group = _groupViewFactory.Make(GetGroup(chat.Group.Id));

                // Group chat users get some historic messages for context
                chatDetails.Messages = chat
                                        .Messages
                                        .OrderByDescending(x => x.Timestamp)
                                        .Take(30)
                                        .Select(x => new
                                        {
                                            x.Id,
                                            Type = "usermessage",
                                            ChatId = chat.Id,
                                            x.Timestamp,
                                            x.Message,
                                            FromUser = _userViewFactory.Make(chatMessageUsers.Single(y => y.Id == x.User.Id))
                                        });
            }

            _backChannelService.NotifyChatJoinedToUserChannel(userId, chatDetails);
        }

        private All_Groups.Result GetGroup(string groupId)
        {
            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == groupId)
                .ToList()
                .First();
        }

        #endregion     
    
    }
}