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
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ChatUpdated(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IUserViewModelBuilder userViewModelBuilder,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _userViewModelBuilder = userViewModelBuilder;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Chat> domainEvent)
        {
            // Get chat
            var chat = domainEvent.DomainModel;

            // Add all users to chat channel
            var users = _documentSession.Load<User>(chat.Users.Select(x => x.Id)).ToList();
            foreach (var connectionId in users.SelectMany(x => x.Sessions.Select(y => y.ConnectionId)))
            {
                _userContext.AddUserToChatChannel(chat.Id, connectionId);
            }

            // Notify initiator they have joined chat
            dynamic chatDetails = new ExpandoObject();

            chatDetails.ChatId = chat.Id;
            chatDetails.Users = users.Select(_userViewFactory.Make);
            chatDetails.Messages = new object[]{};

            if (chat.ChatType == "group")
            {
                chatDetails.Group = _groupViewFactory.Make(GetGroup(chat.Group.Id));
            }
            
            _userContext.GetUserChannel(domainEvent.User.Id).chatJoined(chatDetails);
        }

        public void Handle(UserJoinedChatEvent domainEvent)
        {
            // Get chat
            var chat = domainEvent.Chat;

            // Add user to chat channel
            foreach (var session in domainEvent.User.Sessions)
            {
                _userContext.AddUserToChatChannel(chat.Id, session.ConnectionId);
            }

            // Notify user they have joined chat
            var users = _documentSession.Load<User>(chat.Users.Select(x => x.Id));
            var chatMessageUsers = _documentSession.Load<User>(chat.Messages.Select(x => x.User.Id).Distinct());
            
            dynamic chatDetails = new ExpandoObject();

            chatDetails.ChatId = chat.Id;
            chatDetails.Users = users.Select(_userViewFactory.Make);
            chatDetails.Messages = chat
                                    .Messages
                                    .OrderByDescending(x => x.Timestamp)
                                    .Take(30)
                                    .Select(x => new 
                                        {
                                            x.Id,
                                            ChatId = chat.Id,
                                            x.Timestamp,
                                            x.Message,
                                            FromUser = _userViewFactory.Make(chatMessageUsers.Single(y => y.Id == x.User.Id))
                                        });

            if (chat.ChatType == "group")
            {
                chatDetails.Group = _groupViewFactory.Make(GetGroup(chat.Group.Id));
            }
            
            _userContext.GetUserChannel(domainEvent.User.Id).chatJoined(chatDetails);

            // Notify all users in chat that new user has joined
            var details = new
            {
                ChatId = chat.Id,
                User = _userViewFactory.Make(domainEvent.User)
            };
            _userContext.GetChatChannel(chat.Id).userJoinedChat(details);
        }

        public void Handle(UserExitedChatEvent domainEvent)
        {
            // Get chat
            var chat = domainEvent.Chat;

            // Remove user from chat channel
            foreach (var session in domainEvent.User.Sessions)
            {
                _userContext.RemoveUserFromChatChannel(chat.Id, session.ConnectionId);
            }

            // Notify all users in chat that user has left chat
            var details = new
            {
                ChatId = chat.Id,
                User = _userViewFactory.Make(domainEvent.User)
            };
            _userContext.GetChatChannel(chat.Id).userExitedChat(details);
        }

        public void Handle(DomainModelCreatedEvent<ChatMessage> domainEvent)
        {
            // Get chat
            var chat = domainEvent.Sender as Chat;

            // Get message
            var chatMessage = domainEvent.DomainModel;

            // Send message to clients
            var messageSender = _documentSession.Load<User>(chatMessage.User.Id);
            var users = _documentSession.Load<User>(chat.Users.Select(x => x.Id)).ToList();

            dynamic chatMessageDetails = new ExpandoObject();

            chatMessageDetails.Id = chatMessage.Id;
            chatMessageDetails.ChatId = chat.Id;
            chatMessageDetails.Timestamp = chatMessage.Timestamp;
            chatMessageDetails.FromUser = _userViewFactory.Make(messageSender);
            chatMessageDetails.Message = chatMessage.Message;

            // If this is a private chat, and its the first message returned, then send the user list to all invitees as they don't have it yet
            if (chat.ChatType == "private" && chat.Messages.Count() == 1)
            {
                chatMessageDetails.Users = users.Select(_userViewFactory.Make);
            }

            _userContext.GetChatChannel(chat.Id).chatMessageReceived(chatMessageDetails);
        }

        private Group GetGroup(string groupId)
        {
            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == groupId)
                .ToList()
                .Select(x => x.Group)
                .First();
        }

        #endregion     
    
    }
}