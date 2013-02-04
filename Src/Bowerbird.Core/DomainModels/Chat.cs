/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System.Linq;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Chat : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        protected Chat() 
            : base()
        {
            InitMembers();
        }

        public Chat(
            string id,
            User createdByUser,
            IEnumerable<User> users,
            DateTime createdDateTime,
            Group group)
            : base()
        {
            InitMembers();

            Id = id;
            CreatedByUser = createdByUser;
            CreatedDateTime = createdDateTime;
            UpdatedDateTime = createdDateTime;

            if (group != null)
            {
                Group = group;
            }

            foreach (var user in users)
            {
                SetUser(user);
            }

            ApplyEvent(new DomainModelCreatedEvent<Chat>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference CreatedByUser { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public DateTime UpdatedDateTime { get; private set; }

        public IEnumerable<DenormalisedUserReference> Users { get; private set; }

        public DenormalisedGroupReference Group { get; private set; }

        public IEnumerable<ChatMessage> Messages { get; private set; }

        public string ChatType
        {
            get { return Group != null ? "group" : "private"; }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Users = new List<DenormalisedUserReference>();
            Messages = new List<ChatMessage>();
        }

        private User SetUser(User user)
        {
            if (Users.All(x => x.Id != user.Id))
            {
                ((List<DenormalisedUserReference>)Users).Add(user);

                return user;
            }

            return null;
        }

        public Chat AddMessage(User user, DateTime timestamp, string messageId, string message)
        {
            var chatMessage = new ChatMessage(messageId, user, timestamp, message);

            ((List<ChatMessage>)Messages).Add(chatMessage);

            ApplyEvent(new DomainModelCreatedEvent<ChatMessage>(chatMessage, user, this));

            return this;
        }

        public Chat AddUser(User user)
        {
            SetUser(user);
            UpdatedDateTime = DateTime.UtcNow;

            // Irrespective of whether user already was in chat or not, we need to fire this event so that the user client is notified
            // that the chat has begun.
            ApplyEvent(new UserJoinedChatEvent(user, this, this));

            return this;
        }

        public Chat RemoveUser(User user)
        {
            ((List<DenormalisedUserReference>)Users).RemoveAll(x => x.Id == user.Id);
            UpdatedDateTime = DateTime.UtcNow;

            ApplyEvent(new UserExitedChatEvent(user, this, this));

            return this;
        }

        #endregion
    }
}