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

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Sends a chat message to clients. The situations in which this can occur are:
    /// - A new chat is created
    /// - A chat message is added to an existing chat
    /// </summary>
    public class ChatMessageAdded : 
        DomainEventHandlerBase, 
        IEventHandler<DomainModelCreatedEvent<Chat>>, 
        IEventHandler<DomainModelCreatedEvent<ChatMessage>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ChatMessageAdded(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IUserViewModelBuilder userViewModelBuilder,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _userViewModelBuilder = userViewModelBuilder;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Chat> domainEvent)
        {
            var chat = domainEvent.DomainModel;
            var allUsers = _documentSession.Load<User>(chat.Users.Select(x => x.Id)).ToList();
            var firstChatMessage = chat.Messages.FirstOrDefault();
            var messageSender = _documentSession.Load<User>(firstChatMessage.User.Id);

            // Get all clientIds for all users that will receive message and add to the channel
            foreach (var connectionId in allUsers.SelectMany(x => x.Sessions.Select(y => y.ConnectionId)))
            {
                _userContext.AddUserToChatChannel(chat.Id, connectionId);
            }

            if (firstChatMessage != null)
            {
                SendChatMessage(chat, firstChatMessage, messageSender, allUsers);
            }
        }

        public void Handle(DomainModelCreatedEvent<ChatMessage> domainEvent)
        {
            var chat = domainEvent.Sender as Chat;
            var chatMessage = domainEvent.DomainModel;
            var allUsers = _documentSession.Load<User>(chat.Users.Select(x => x.Id)).ToList();
            var messageSender = _documentSession.Load<User>(chatMessage.User.Id);

            SendChatMessage(chat, chatMessage, messageSender, allUsers);
        }

        private void SendChatMessage(Chat chat, ChatMessage chatMessage, User messageSender, IEnumerable<User> allUsers)
        {
            var message = new
            {
                Id = chatMessage.Id,
                ChatId = chat.Id,
                Timestamp = chatMessage.Timestamp,
                FromUser = _userViewFactory.Make(messageSender),
                Message = chatMessage.Message,
                Users = allUsers.Select(_userViewFactory.Make)
            };

            // Send message to clients
            _userContext.GetChatChannel(chat.Id).chatMessageReceived(message);
        }

        #endregion     
    }
}