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

using System.Collections.Generic;
using System.Linq;
using System;

namespace Bowerbird.Web.Chat.Models
{
    public interface IChatRepository : IDisposable
    {        
        IList<ChatGroup> Groups { get; }
        IList<ChatUser> Users { get; }
        IList<ChatGroup> GetUserGroups(string userId);
        
        //IQueryable<ChatUser> SearchUsers(string name);
        IList<ChatMessage> GetMessagesByGroup(string groupId);
        IList<ChatMessage> GetPreviousMessages(string messageId);
        //IQueryable<ChatGroup> GetAllowedRooms(ChatUser user);
        ChatMessage GetMessagesById(string id);

        ChatUser GetUserById(string userId);
        ChatGroup GetGroupById(string roomName);
        //ChatUser GetUserByName(string userName);
        ChatUser GetUserByClientId(string clientId);
        //ChatUser GetUserByIdentity(string userIdentity);

        ChatClient GetClientById(string clientId);

        void Add(ChatClient client);
        void Add(ChatMessage message);
        //void Add(ChatGroup @group);
        //void Add(ChatUser user);
        void Remove(ChatClient client);
        //void Remove(ChatGroup @group);
        //void Remove(ChatUser user);
        //void RemoveAllClients();
        void CommitChanges();
    }
}