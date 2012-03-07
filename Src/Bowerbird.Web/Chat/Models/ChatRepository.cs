/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia

Bowerbird.Web.Chat and sub namespaces have the following attribution as sourced from https://github.com/davidfowl/JabbR/

- Copyright (c) 2011 David Fowler
- Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
- The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Web.Config;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Chat.Models
{
    public class PersistedRepository : IChatRepository
    {
        private readonly IDocumentSession _documentSession;
        private readonly IUserContext _userContext;

        public PersistedRepository(
            IDocumentSession documentSession,
            IUserContext userContext
            )
        {
            _documentSession = documentSession;
            _userContext = userContext;
        }

        // get groups for the contextual user
        public IList<ChatGroup> Groups
        {
            get
            {
                // get all the memberships the user has
                var groupMemberships = _documentSession
                    .Query<GroupMember>()
                    .Include(x => x.Group.Id)
                    .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                    .ToList()
                    .Select(x => x.Group.Id);

                var groups = _documentSession
                    .Query<Group>()
                    .Where(x => x.Id.In(groupMemberships))
                    .ToList();

                return
                    groups.Select(x => new ChatGroup()
                                           {
                                               Name = x.Name
                                           }) as IList<ChatGroup>;
            }
        }

        public IList<ChatUser> Users 
        { 
            get
            {
                return _documentSession.Query<User>()
                       .Select(x => new ChatUser()
                                        {
                                            Id = x.Id,
                                            Name = x.GetName(),
                                        }) as IList<ChatUser>;  
            } 
        }

        public IList<ChatGroup> GetUserGroups(string userId)
        {
                // get all the memberships the user has
                var groupMemberships = _documentSession
                    .Query<GroupMember>()
                    .Include(x => x.Group.Id)
                    .Where(x => x.User.Id == userId)
                    .ToList()
                    .Select(x => x.Group.Id);

                var groups = _documentSession
                    .Query<Group>()
                    .Where(x => x.Id.In(groupMemberships))
                    .ToList();

                return
                    groups.Select(x => new ChatGroup()
                                           {
                                               Name = x.Name
                                           }) as IList<ChatGroup>;
        }

        public IList<ChatMessage> GetMessagesByGroup(string groupId)
        {
            var chatMessages = _documentSession.Query<GroupChatMessage>()
                .Include(x => x.Group.Id)
                .Where(x => x.Group.Id == groupId)
                .OrderByDescending(x => x.When)
                .ToList();

            return chatMessages.Select(x => new ChatMessage()
                                                {
                                                    Content = x.Content,
                                                    Group = new ChatGroup() {Name = x.Group.Name},
                                                    Id = x.Id,
                                                    User = new ChatUser() {Name = x.User.FirstName + " " + x.User.LastName}
                                                }) as IList<ChatMessage>;
        }

        public IList<ChatMessage> GetPreviousMessages(string messageId)
        {
            var info = _documentSession
                .Query<GroupChatMessage>()
                .Include(m => m.Group.Id)
                .Where(x => x.Id == messageId)
                .Select(
                    x => new {
                                x.When,
                                GroupId = x.Group.Id
                            }
                ).FirstOrDefault();

            return  GetMessagesByGroup(info.GroupId)
                        .Where(x => x.When > info.When)
                        .Select(x => new ChatMessage()
                                         {
                                             Content = x.Content,
                                             Group = new ChatGroup(){Name = x.Group.Name},
                                             Id = x.Id,
                                             User = new ChatUser(){Name = x.User.Name}
                                         }) as IList<ChatMessage>;

        }

        //public IQueryable<ChatUser> Users
        //{
        //    get { return _documentSession.Users; }
        //}

        //public void Add(ChatGroup room)
        //{
        //    _documentSession.Rooms.Add(room);
        //    _documentSession.SaveChanges();
        //}

        //public void Add(ChatUser user)
        //{
        //    _documentSession.Users.Add(user);
        //    _documentSession.SaveChanges();
        //}

        public void Add(ChatMessage message)
        {
            var groupChatMessage = new GroupChatMessage()
                                       {
                                           Content = message.Content,
                                           Group = _documentSession.Load<Group>(message.Group.Id),
                                           User = _documentSession.Load<User>(message.User.Id),
                                           When = message.When
                                       };

            _documentSession.Store(groupChatMessage);
        }

        //public void Remove(ChatGroup room)
        //{
        //    _documentSession.Rooms.Remove(room);
        //    _documentSession.SaveChanges();
        //}

        //public void Remove(ChatUser user)
        //{
        //    _documentSession.Users.Remove(user);
        //    _documentSession.SaveChanges();
        //}

        public void CommitChanges()
        {
            _documentSession.SaveChanges();
        }

        public void Dispose()
        {
            _documentSession.Dispose();
        }

        public ChatUser GetUserById(string userId)
        {
            var user = _documentSession.Load<User>(userId);

            return new ChatUser() 
            {
                Id = user.Id,
                Name = user.GetName()
            };
        }

        //public ChatUser GetUserByName(string userName)
        //{
        //    return _documentSession.Users.FirstOrDefault(u => u.Name == userName);
        //}

        public ChatGroup GetGroupById(string groupId)
        {
            var group = _documentSession.Load<Group>(groupId);

            return new ChatGroup()
                       {
                           Id = group.Id,
                           Name = group.Name,
                           Messages = 
                           _documentSession.Query<GroupChatMessage>()
                               .Where(x => x.Group.Id == groupId)
                               .Select(y => new ChatMessage()
                                                {
                                                    Content = y.Content,
                                                    Id = y.Id,
                                                    When = y.When
                                                }) as ICollection<ChatMessage>
                       };

        }

        public ChatMessage GetMessagesById(string id)
        {
            var message = _documentSession.Load<GroupChatMessage>(id);

            return new ChatMessage()
                       {
                           Content = message.Content,
                           Group = new ChatGroup()
                                       {
                                           Name = message.Group.Name,
                                           Id = message.Group.Id
                                       }
                       };
        }

        //public IQueryable<ChatGroup> GetAllowedRooms(ChatUser user)
        //{
        //    // All *open* public and private rooms the user can see.
        //    return _documentSession.Rooms
        //        .Where(r =>
        //               (!r.Private && !r.Closed) ||
        //               (r.Private && !r.Closed && r.AllowedUsers.Any(u => u.Key == user.Key)));
        //}


        //public IQueryable<ChatUser> SearchUsers(string name)
        //{
        //    return _documentSession.Users.Online().Where(u => u.Name.Contains(name));
        //}

        public void Add(ChatClient client)
        {
            var clientSession = new ClientSession
            (
                _documentSession.Load<User>(client.User.Id),
                client.Id,
                DateTime.Now
            );
            
            _documentSession.Store(clientSession);
            _documentSession.SaveChanges();
        }

        public void Remove(ChatClient client)
        {
            var clientSession = _documentSession.Load<ClientSession>(client.Id);

            _documentSession.Delete(clientSession);
            _documentSession.SaveChanges();
        }

        public ChatUser GetUserByClientId(string clientId)
        {
            var client = GetClientById(clientId);
            if (client != null)
            {
                return client.User;
            }
            return null;
        }

        //public ChatUser GetUserByIdentity(string userIdentity)
        //{
        //    return _documentSession.Users.FirstOrDefault(u => u.Identity == userIdentity);
        //}

        public ChatClient GetClientById(string clientId)
        {
            var client = _documentSession.Load<ClientSession>(clientId);

            return new ChatClient()
                       {
                           Id = client.ClientId,
                           User = new ChatUser()
                           {
                               Id = client.User.Id,
                               Name = client.User.FirstName + ' ' + client.User.LastName
                           }
                       };
        }

        //public void RemoveAllClients()
        //{
        //    foreach (var c in _documentSession.Clients)
        //    {
        //        _documentSession.Clients.Remove(c);
        //    }
        //}
    }
}