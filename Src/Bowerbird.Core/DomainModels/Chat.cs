﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Dynamic;
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

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        #endregion

        #region Constructors

        protected Chat() 
            : base()
        {
            InitMembers();

            EnableEvents();
        }

        public Chat(
            string id,
            User createdByUser,
            IEnumerable<User> users,
            DateTime createdDateTime)
            : base()
        {
            InitMembers();

            Id = id;
            CreatedByUser = createdByUser;
            CreatedDateTime = createdDateTime;

            foreach (var user in users)
            {
                AddUser(user);
            }

            EnableEvents();
            FireEvent(new DomainModelCreatedEvent<Chat>(this, createdByUser, this));
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
            Group = group;

            foreach (var user in users)
            {
                AddUser(user);
            }

            EnableEvents();
            FireEvent(new DomainModelCreatedEvent<Chat>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference CreatedByUser { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

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

        public Chat AddMessage(User user, DateTime timestamp, string message)
        {
            var chatMessage = new ChatMessage(user, timestamp, message);

            ((List<ChatMessage>)Messages).Add(chatMessage);

            FireEvent(new DomainModelCreatedEvent<ChatMessage>(chatMessage, user, this));

            return this;
        }

        public Chat AddUser(User user)
        {
            if(Users.All(x => x.Id != user.Id))
            {
                ((List<DenormalisedUserReference>)Users).Add(user);
            }

            FireEvent(new UserJoinedChatEvent(user, this, this));

            return this;
        }

        public Chat RemoveUser(User user)
        {
            ((List<DenormalisedUserReference>)Users).RemoveAll(x => x.Id == user.Id);

            FireEvent(new UserExitedChatEvent(user, this, this));

            return this;
        }

        #endregion
    }
}