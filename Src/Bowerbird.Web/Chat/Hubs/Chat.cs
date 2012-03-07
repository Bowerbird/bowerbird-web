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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Bowerbird.Web.Chat.ContentProviders;
using Bowerbird.Web.Chat.Infrastructure;
using Bowerbird.Web.Chat.Models;
using Bowerbird.Web.Chat.Services;
using Bowerbird.Web.Chat.ViewModels;
using Newtonsoft.Json;
using SignalR.Hubs;

namespace Bowerbird.Web.Chat.Hubs
{
    public class Chat : Hub, IDisconnect, INotificationService
    {
        private readonly IChatRepository _repository;
        private readonly IChatService _service;
        private readonly IResourceProcessor _resourceProcessor;

        public Chat(
            IResourceProcessor resourceProcessor, 
            IChatService service, 
            IChatRepository repository
            )
        {
            _resourceProcessor = resourceProcessor;
            _service = service;
            _repository = repository;
        }

        private string UserAgent
        {
            get
            {
                if (Context.Headers != null)
                {
                    return Context.Headers["User-Agent"];
                }
                return null;
            }
        }

        private bool OutOfSync
        {
            get
            {
                string version = Caller.version;
                return String.IsNullOrEmpty(version) ||
                        new Version(version) != typeof(Chat).Assembly.GetName().Version;
            }
        }

        public bool Join()
        {
            SetVersion();

            // Get the client state
            ClientState clientState = GetClientState();

            // Try to get the user from the client state
            ChatUser user = _repository.GetUserById(clientState.UserId);

            // Threre's no user being tracked
            if (user == null)
            {
                return false;
            }

            // Update some user values
            _service.UpdateActivity(user, Context.ConnectionId, UserAgent);
            _repository.CommitChanges();

            OnUserInitialize(clientState, user);

            return true;
        }

        private void SetVersion()
        {
            // Set the version on the client
            Caller.version = typeof(Chat).Assembly.GetName().Version.ToString();
        }

        public bool CheckStatus()
        {
            bool outOfSync = OutOfSync;

            SetVersion();

            string id = Caller.id;

            ChatUser user = _repository.VerifyUserId(id);

            // Make sure this client is being tracked
            _service.AddClient(user, Context.ConnectionId, UserAgent);

            var currentStatus = (UserStatus)user.Status;

            if (currentStatus == UserStatus.Offline)
            {
                // Mark the user as inactive
                user.Status = (int)UserStatus.Inactive;
                _repository.CommitChanges();

                // If the user was offline that means they are not in the user list so we need to tell
                // everyone the user is really in the room
                var userViewModel = new UserViewModel(user);

                foreach (var group in user.Groups)
                {
                    var isOwner = user.OwnedGroups.Contains(group);

                    // Tell the people in this room that you've joined
                    Clients[group.Id].addUser(userViewModel, group.Id, isOwner).Wait();

                    // Update the room count
                    OnRoomChanged(group);

                    // Add the caller to the group so they receive messages
                    GroupManager.AddToGroup(Context.ConnectionId, group.Id).Wait();
                }
            }

            return outOfSync;
        }

        private void OnUserInitialize(ClientState clientState, ChatUser user)
        {
            // Update the active room on the client (only if it's still a valid room)
            if (user.Groups.Any(group => group.Id.Equals(clientState.ActiveGroupId, StringComparison.OrdinalIgnoreCase)))
            {
                // Update the active room on the client (only if it's still a valid room)
                Caller.activeGroupId = clientState.ActiveGroupId;
            }

            LogOn(user, Context.ConnectionId);
        }

        public bool Send(string content, string groupId)
        {
            var message = new ClientMessage
            {
                Content = content,
                Id = Guid.NewGuid().ToString("d"),
                GroupId = groupId
            };

            return Send(message);
        }

        public bool Send(ClientMessage message)
        {
            bool outOfSync = OutOfSync;

            SetVersion();

            // Sanitize the content (strip and bad html out)
            message.Content = HttpUtility.HtmlEncode(message.Content);

            // See if this is a valid command (starts with /)
            //if (TryHandleCommand(message.Content, message.GroupId))
            //{
            //    return outOfSync;
            //}

            string id = Caller.id;

            ChatUser user = _repository.VerifyUserId(id);
            ChatGroup group = _repository.VerifyUserGroup(user, message.GroupId);

            // Update activity *after* ensuring the user, this forces them to be active
            UpdateActivity(user, group);

            HashSet<string> links;
            var messageText = ParseChatMessageText(message.Content, out links);

            ChatMessage chatMessage = _service.AddMessage(user, group, message.Id, messageText);

            var messageViewModel = new MessageViewModel(chatMessage);
            Clients[group.Id].addMessage(messageViewModel, group.Id);

            _repository.CommitChanges();

            string clientMessageId = chatMessage.Id;

            // Update the id on the message
            chatMessage.Id = Guid.NewGuid().ToString("d");
            _repository.CommitChanges();

            if (!links.Any())
            {
                return outOfSync;
            }

            ProcessUrls(links, group.Id, clientMessageId, chatMessage.Id);

            return outOfSync;
        }

        private string ParseChatMessageText(string content, out HashSet<string> links)
        {
            var textTransform = new TextTransform(_repository);
            string message = textTransform.Parse(content);
            return TextTransform.TransformAndExtractUrls(message, out links);
        }

        public UserViewModel GetUserInfo()
        {
            string id = Caller.id;

            ChatUser user = _repository.VerifyUserId(id);

            return new UserViewModel(user);
        }

        public Task Disconnect()
        {
            DisconnectClient(Context.ConnectionId);

            return null;
        }

        public IEnumerable<GroupViewModel> GetGroups()
        {
            string id = Caller.id;
            ChatUser user = _repository.VerifyUserId(id);

            var groups = _repository.GetUserGroups(user.Id).Select(r => new GroupViewModel
            {
                Name = r.Name,
                Count = r.Users.Count(u => u.Status != (int)UserStatus.Offline)
            });

            return groups;
        }

        public IEnumerable<MessageViewModel> GetPreviousMessages(string messageId)
        {
            var previousMessages = (from m in _repository.GetPreviousMessages(messageId)
                                    orderby m.When descending
                                    select m).Take(100);


            return previousMessages.AsEnumerable()
                                   .Reverse()
                                   .Select(m => new MessageViewModel(m));
        }

        public GroupViewModel GetGroupInfo(string groupId)
        {
            if (String.IsNullOrEmpty(groupId))
            {
                return null;
            }

            ChatGroup group = _repository.GetGroupById(groupId);

            if (group == null)
            {
                return null;
            }

            var recentMessages = (from m in _repository.GetMessagesByGroup(groupId)
                                  orderby m.When descending
                                  select m).Take(30);

            return new GroupViewModel
            {
                Name = group.Name,
                //Users = group.Users.Where(x => x.Status != (int)UserStatus.Offline),
                //Owners = from u in room.Owners.Online()
                //         select u.Name,
                RecentMessages = recentMessages.AsEnumerable().Reverse().Select(m => new MessageViewModel(m)),
                //Topic = ConvertUrlsAndRoomLinks(room.Topic ?? "")
            };
        }

        private string ConvertUrlsAndRoomLinks(string message)
        {
            TextTransform textTransform = new TextTransform(_repository);
            message = textTransform.ConvertHashtagsToRoomLinks(message);
            HashSet<string> urls;
            return TextTransform.TransformAndExtractUrls(message, out urls);
        }

        public void Typing()
        {
            string groupId = Caller.activeGroupId;

            Typing(groupId);
        }

        public void Typing(string groupId)
        {
            string id = Caller.id;
            ChatUser user = _repository.GetUserById(id);

            if (user == null)
            {
                return;
            }

            ChatGroup group = _repository.VerifyUserGroup(user, groupId);

            UpdateActivity(user, group);
            var userViewModel = new UserViewModel(user);
            Clients[group.Id].setTyping(userViewModel, group.Id);
        }

        private void LogOn(ChatUser user, string clientId)
        {
            // Update the client state
            Caller.id = user.Id;
            Caller.name = user.Name;

            var userViewModel = new UserViewModel(user);
            var groups = new List<GroupViewModel>();

            foreach (var group in user.Groups)
            {
                var isOwner = user.OwnedGroups.Contains(group);

                // Tell the people in this room that you've joined
                Clients[group.Id].addUser(userViewModel, group.Id, isOwner).Wait();

                // Update the room count
                OnRoomChanged(group);

                // Add the caller to the group so they receive messages
                GroupManager.AddToGroup(clientId, group.Id).Wait();

                // Add to the list of group names
                groups.Add(new GroupViewModel
                {
                    Name = group.Name,
                    Id = group.Id
                });
            }

            // Initialize the chat with the rooms the user is in
            Caller.logOn(groups);
        }

        private void UpdateActivity(ChatUser user, ChatGroup room)
        {
            UpdateActivity(user);

            OnUpdateActivity(user, room);
        }

        private void UpdateActivity(ChatUser user)
        {
            _service.UpdateActivity(user, Context.ConnectionId, UserAgent);

            _repository.CommitChanges();
        }

        private void ProcessUrls(IEnumerable<string> links, string roomName, string clientMessageId, string messageId)
        {
            var contentTasks = links.Select(_resourceProcessor.ExtractResource).ToArray();
            Task.Factory.ContinueWhenAll(contentTasks, tasks =>
            {
                foreach (var task in tasks)
                {
                    if (task.IsFaulted)
                    {
                        Trace.TraceError(task.Exception.GetBaseException().Message);
                        continue;
                    }

                    if (task.Result == null || String.IsNullOrEmpty(task.Result.Content))
                    {
                        continue;
                    }

                    // Try to get content from each url we're resolved in the query
                    string extractedContent = "<p>" + task.Result.Content + "</p>";

                    // Notify the room
                    Clients[roomName].addMessageContent(clientMessageId, extractedContent, roomName);

                    _service.AppendMessage(messageId, extractedContent);
                }
            });
        }

        private void DisconnectClient(string clientId)
        {
            ChatUser user = _service.DisconnectClient(clientId);

            // There's no associated user for this client id
            if (user == null)
            {
                return;
            }

            // The user will be marked as offline if all clients leave
            if (user.Status == (int)UserStatus.Offline)
            {
                foreach (var group in user.Groups)
                {
                    var userViewModel = new UserViewModel(user);

                    Clients[group.Id].leave(userViewModel, group.Id).Wait();

                    OnRoomChanged(group);
                }
            }
        }

        private void OnUpdateActivity(ChatUser user, ChatGroup room)
        {
            var userViewModel = new UserViewModel(user);
            Clients[room.Name].updateActivity(userViewModel, room.Name);
        }

        private void LeaveRoom(ChatUser user, ChatGroup room)
        {
            var userViewModel = new UserViewModel(user);
            Clients[room.Name].leave(userViewModel, room.Name).Wait();

            foreach (var client in user.ConnectedClients)
            {
                GroupManager.RemoveFromGroup(client.Id, room.Name).Wait();
            }

            OnRoomChanged(room);
        }

        void INotificationService.LogOn(ChatUser user, string clientId)
        {
            LogOn(user, clientId);
        }

        void INotificationService.JoinRoom(ChatUser user, ChatGroup group)
        {
            var userViewModel = new UserViewModel(user);
            var groupViewModel = new GroupViewModel
            {
                Name = group.Name,
                Id = group.Id
            };

            var isOwner = user.OwnedGroups.Contains(group);

            // Tell all clients to join this room
            foreach (var client in user.ConnectedClients)
            {
                Clients[client.Id].joinRoom(groupViewModel);
            }

            // Tell the people in this room that you've joined
            Clients[group.Id].addUser(userViewModel, group.Id, isOwner).Wait();

            // Notify users of the room count change
            OnRoomChanged(group);

            foreach (var client in user.ConnectedClients)
            {
                // Add the caller to the group so they receive messages
                GroupManager.AddToGroup(client.Id, group.Id).Wait();
            }
        }

        void INotificationService.PostNotification(ChatGroup room, ChatUser user, string message)
        {
            foreach (var client in user.ConnectedClients)
            {
                Clients[client.Id].postNotification(message, room.Name);
            }
        }

        void INotificationService.ListRooms(ChatUser user)
        {
            string userId = Caller.id;
            var userModel = new UserViewModel(user);

            Caller.showUsersRoomList(userModel, user.Groups.Allowed(userId).Select(r => r.Name));
        }

        void INotificationService.ListUsers()
        {
            var users = _repository.Users.Online().Select(s => s.Name).OrderBy(s => s);
            Caller.listUsers(users);
        }

        void INotificationService.ListUsers(IEnumerable<ChatUser> users)
        {
            Caller.listUsers(users.Select(s => s.Name));
        }

        void INotificationService.ListUsers(ChatGroup room, IEnumerable<string> names)
        {
            Caller.showUsersInRoom(room.Name, names);
        }

        void INotificationService.LogOut(ChatUser user, string clientId)
        {
            DisconnectClient(clientId);

            var rooms = user.Groups.Select(r => r.Name);

            Caller.logOut(rooms);
        }

        void INotificationService.ShowUserInfo(ChatUser user)
        {
            string userId = Caller.id;

            Caller.showUserInfo(new
            {
                Name = user.Name,
                OwnedRooms = user.OwnedGroups
                    .Allowed(userId)
                    .Where(r => !r.Closed)
                    .Select(r => r.Name),
                Status = ((UserStatus)user.Status).ToString(),
                LastActivity = user.LastActivity,
                Rooms = user.Groups.Allowed(userId).Select(r => r.Name)
            });
        }

        void INotificationService.ShowRooms()
        {
            Caller.showRooms(GetGroups());
        }

        private void OnRoomChanged(ChatGroup group)
        {
            var groupViewModel = new GroupViewModel
            {
                Name = group.Name,
                Id = group.Id
            };

            // Update the room count
            Clients.updateRoomCount(groupViewModel, group.Users.Online().Count());
        }

        private ClientState GetClientState()
        {
            // New client state
            var chatState = GetCookieValue("bowerbird.state");

            ClientState clientState = null;

            if (String.IsNullOrEmpty(chatState))
            {
                clientState = new ClientState();
            }
            else
            {
                clientState = JsonConvert.DeserializeObject<ClientState>(chatState);
            }

            // Read the id from the caller if there's no cookie
            clientState.UserId = clientState.UserId ?? Caller.id;

            return clientState;
        }

        private string GetCookieValue(string key)
        {
            string value = Context.Cookies[key];
            return value != null ? HttpUtility.UrlDecode(value) : null;
        }
    }
}