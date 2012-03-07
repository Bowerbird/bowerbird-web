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
using System.Text.RegularExpressions;
using Bowerbird.Web.Chat.Infrastructure;
using Bowerbird.Web.Chat.Models;

namespace Bowerbird.Web.Chat.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;

        private const int NoteMaximumLength = 140;
        private const int TopicMaximumLength = 80;

        public ChatService(
            IChatRepository repository
            )
        {
            _repository = repository;
        }

        public void UpdateActivity(ChatUser user, string clientId, string userAgent)
        {
            user.Status = (int)UserStatus.Active;
            user.LastActivity = DateTime.UtcNow;

            ChatClient client = AddClient(user, clientId, userAgent);
            client.UserAgent = userAgent;
            client.LastActivity = DateTimeOffset.UtcNow;
        }

        public ChatMessage AddMessage(ChatUser user, ChatGroup @group, string id, string content)
        {
            var chatMessage = new ChatMessage
            {
                Id = id,
                User = user,
                Content = content,
                When = DateTimeOffset.UtcNow,
                Group = group
            };

            _repository.Add(chatMessage);

            return chatMessage;
        }

        public void AppendMessage(string id, string content)
        {
            ChatMessage message = _repository.GetMessagesById(id);

            message.Content += content;

            _repository.CommitChanges();
        }

        // take this
        //public void AddOwner(ChatUser ownerOrCreator, ChatUser targetUser, ChatGroup targetGroup)
        //{
        //    // Ensure the user is owner of the target room
        //    EnsureOwner(ownerOrCreator, targetGroup);

        //    if (targetGroup.Owners.Contains(targetUser))
        //    {
        //        // If the target user is already an owner, then throw
        //        throw new InvalidOperationException(String.Format("'{0}' is already an owner of '{1}'.", targetUser.Name, targetGroup.Name));
        //    }

        //    // Make the user an owner
        //    targetGroup.Owners.Add(targetUser);
        //    targetUser.OwnedGroups.Add(targetGroup);

        //    if (targetGroup.Private)
        //    {
        //        if (!targetGroup.AllowedUsers.Contains(targetUser))
        //        {
        //            // If the room is private make this user allowed
        //            targetGroup.AllowedUsers.Add(targetUser);
        //            targetUser.AllowedGroups.Add(targetGroup);
        //        }
        //    }
        //}

        //// take this
        //public void RemoveOwner(ChatUser creator, ChatUser targetUser, ChatGroup targetGroup)
        //{
        //    // Ensure the user is creator of the target room
        //    EnsureCreator(creator, targetGroup);

        //    if (!targetGroup.Owners.Contains(targetUser))
        //    {
        //        // If the target user is not an owner, then throw
        //        throw new InvalidOperationException(String.Format("'{0}' is not an owner of '{1}'.", targetUser.Name, targetGroup.Name));
        //    }

        //    // Remove user as owner of room
        //    targetGroup.Owners.Remove(targetUser);
        //    targetUser.OwnedGroups.Remove(targetGroup);
        //}

        //// take this
        //public void KickUser(ChatUser user, ChatUser targetUser, ChatGroup targetGroup)
        //{
        //    EnsureOwner(user, targetGroup);

        //    if (targetUser == user)
        //    {
        //        throw new InvalidOperationException("Why would you want to kick yourself?");
        //    }

        //    if (!IsUserInRoom(targetGroup, targetUser))
        //    {
        //        throw new InvalidOperationException(String.Format("'{0}' isn't in '{1}'.", targetUser.Name, targetGroup.Name));
        //    }

        //    // If this user isnt' the creator and the target user is an owner then throw
        //    if (targetGroup.Creator != user && targetGroup.Owners.Contains(targetUser))
        //    {
        //        throw new InvalidOperationException("Owners cannot kick other owners. Only the room creator and kick an owner.");
        //    }

        //    LeaveRoom(targetUser, targetGroup);
        //}

        // take this
        public ChatClient AddClient(ChatUser user, string clientId, string userAgent)
        {
            ChatClient client = _repository.GetClientById(clientId);
            if (client != null)
            {
                return client;
            }

            client = new ChatClient
            {
                Id = clientId,
                User = user,
                UserAgent = userAgent,
                LastActivity = DateTimeOffset.UtcNow
            };

            _repository.Add(client);
            _repository.CommitChanges();

            return client;
        }

        public ChatUser DisconnectClient(string clientId)
        {
            // Remove this client from the list of user's clients
            ChatClient client = _repository.GetClientById(clientId);

            // No client tracking this user
            if (client == null)
            {
                return null;
            }

            // Get the user for this client
            ChatUser user = client.User;

            if (user != null)
            {
                user.ConnectedClients.Remove(client);

                if (!user.ConnectedClients.Any())
                {
                    // If no more clients mark the user as offline
                    user.Status = (int)UserStatus.Offline;
                }

                _repository.Remove(client);
                _repository.CommitChanges();
            }

            return user;
        }

        internal static string NormalizeUserName(string userName)
        {
            return userName.StartsWith("@") ? userName.Substring(1) : userName;
        }

        internal static string NormalizeRoomName(string roomName)
        {
            return roomName.StartsWith("#") ? roomName.Substring(1) : roomName;
        }

        internal static void ThrowUserExists(string userName)
        {
            throw new InvalidOperationException(String.Format("Username {0} already taken, please pick a new one using '/nick nickname'.", userName));
        }

        internal static void ThrowPasswordIsRequired()
        {
            throw new InvalidOperationException("A password is required.");
        }

        internal static bool IsUserInRoom(ChatGroup @group, ChatUser user)
        {
            return group.Users.Any(r => r.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase));
        }

        internal static void ValidateNote(string note, string noteTypeName = "note", int? maxLength = null)
        {
            var lengthToValidateFor = (maxLength ?? NoteMaximumLength);
            if (!String.IsNullOrWhiteSpace(note) &&
                note.Length > lengthToValidateFor)
            {
                throw new InvalidOperationException(
                    String.Format("Sorry, but your {1} is too long. Can please keep it under {0} characters.",
                        lengthToValidateFor, noteTypeName));
            }
        }

        internal static void ValidateTopic(string topic)
        {
            ValidateNote(topic, noteTypeName: "topic", maxLength: TopicMaximumLength);
        }

    }
}