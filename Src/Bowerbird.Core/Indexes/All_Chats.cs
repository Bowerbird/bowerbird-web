///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com

// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au

// Funded by:
// * Atlas of Living Australia

//*/

//using System.Linq;
//using Raven.Abstractions.Indexing;
//using Raven.Client.Indexes;
//using Bowerbird.Core.DomainModels;
//using System.Collections.Generic;

//namespace Bowerbird.Core.Indexes
//{
//    public class All_Chats : AbstractIndexCreationTask<All_Chats.Results>
//    {
//        public class Results
//        {
//            public string ChatId { get; set; }
//            public string[] UserIds { get; set; }

//            public Chat Chat { get; set; }
//            public IEnumerable<User> Users { get; set; }
//        }

//        public All_Chats()
//        {
//            Map<Chat>(chats =>
//                        from chat in chats
//                        select new
//                        {
//                            ChatId = chat.Id,
//                            UserIds = chat.Users
//                        });

//            Store(x => x.ChatId, FieldStorage.Yes);
//            Store(x => x.UserIds, FieldStorage.Yes);
//        }
//    }
//}