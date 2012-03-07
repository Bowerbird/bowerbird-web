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
using Bowerbird.Web.Chat.Services;

namespace Bowerbird.Web.Chat.Models
{
    public static class RepositoryExtensions
    {
        public static IQueryable<ChatUser> Online(this IQueryable<ChatUser> source)
        {
            return source.Where(u => u.Status != (int)UserStatus.Offline);
        }

        public static IEnumerable<ChatUser> Online(this IEnumerable<ChatUser> source)
        {
            return source.Where(u => u.Status != (int)UserStatus.Offline);
        }

        public static IEnumerable<ChatGroup> Allowed(this IEnumerable<ChatGroup> rooms, string userId)
        {
            return from r in rooms
                   where !r.Private ||
                         r.Private && r.AllowedUsers.Any(u => u.Id == userId)
                   select r;
        }

        public static ChatGroup VerifyUserGroup(this IChatRepository repository, ChatUser user, string groupId)
        {
            if (String.IsNullOrEmpty(groupId))
            {
                throw new InvalidOperationException("Use '/join room' to join a room.");
            }

            groupId = ChatService.NormalizeRoomName(groupId);

            ChatGroup @group = repository.GetGroupById(groupId);

            if (group == null)
            {
                throw new InvalidOperationException(String.Format("You're in '{0}' but it doesn't exist.", groupId));
            }

            if (!group.Users.Any(u => u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(String.Format("You're not in '{0}'. Use '/join {0}' to join it.", groupId));
            }

            return group;
        }

        public static ChatUser VerifyUserId(this IChatRepository repository, string userId)
        {
            ChatUser user = repository.GetUserById(userId);

            if (user == null)
            {
                // The user isn't logged in 
                throw new InvalidOperationException("You're not logged in.");
            }

            return user;
        }

        public static ChatGroup VerifyRoom(this IChatRepository repository, string groupId, bool mustBeOpen = true)
        {
            if (String.IsNullOrWhiteSpace(groupId))
            {
                throw new InvalidOperationException("Room name cannot be blank!");
            }

            groupId = ChatService.NormalizeRoomName(groupId);

            var room = repository.GetGroupById(groupId);

            if (room == null)
            {
                throw new InvalidOperationException(String.Format("Unable to find room '{0}'", groupId));
            }

            if (room.Closed && mustBeOpen)
            {
                throw new InvalidOperationException(String.Format("The room '{0}' is closed", groupId));
            }

            return room;
        }

        public static ChatUser VerifyUser(this IChatRepository repository, string userId)
        {
            userId = ChatService.NormalizeUserName(userId);

            ChatUser user = repository.GetUserById(userId);

            if (user == null)
            {
                throw new InvalidOperationException(String.Format("Unable to find user '{0}'.", userId));
            }

            return user;
        }
    }
}