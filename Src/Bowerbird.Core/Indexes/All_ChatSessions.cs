/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com

 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au

 Funded by:
 * Atlas of Living Australia

*/

using System.Linq;
using Bowerbird.Core.DomainModels.Sessions;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class All_ChatSessions : AbstractMultiMapIndexCreationTask<All_ChatSessions.Results>
    {
        public class Results
        {
            public string ClientId { get; set; }
            public string UserId { get; set; }
            public string ChatId { get; set; }
            public int Status { get; set; }
        }

        public All_ChatSessions()
        {
            // when chatting to a private group of users, the chatId is the ChatId
            AddMap<PrivateChatSession>(sessions =>
                from s in sessions
                select new
                {
                    s.ClientId,
                    UserId = s.User.Id,
                    s.Status,
                    s.ChatId
                });

            // when chatting to a public group of users, the chatId is the GroupId
            AddMap<GroupChatSession>(sessions =>
                from s in sessions
                select new
                {
                    s.ClientId,
                    UserId = s.User.Id,
                    s.Status,
                    ChatId = s.Group.Id
                });

            Store(x => x.ClientId, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.ChatId, FieldStorage.Yes);
            Store(x => x.Status, FieldStorage.Yes);
        }
    }
}