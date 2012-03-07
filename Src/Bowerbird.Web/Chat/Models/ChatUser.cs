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
using Bowerbird.Web.Chat.Infrastructure;

namespace Bowerbird.Web.Chat.Models
{
    public class ChatUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime LastActivity { get; set; }
        public int Status { get; set; }

        // List of clients that are currently connected for this user
        public virtual ICollection<ChatClient> ConnectedClients { get; set; }
        public virtual ICollection<ChatGroup> OwnedGroups { get; set; }
        public virtual ICollection<ChatGroup> Groups { get; set; }

        // Private rooms this user is allowed to go into
        public virtual ICollection<ChatGroup> AllowedGroups { get; set; }

        public ChatUser()
        {
            ConnectedClients = new SafeCollection<ChatClient>();
            OwnedGroups = new SafeCollection<ChatGroup>();
            Groups = new SafeCollection<ChatGroup>();
            AllowedGroups = new SafeCollection<ChatGroup>();
        }
    }
}