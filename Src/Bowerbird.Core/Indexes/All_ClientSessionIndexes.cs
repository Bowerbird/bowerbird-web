/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com

 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au

 Funded by:
 * Atlas of Living Australia

*/

using System;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class ClientSessionResults
    {
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime ConnectionCreated { get; set; }
    }

    public class All_ClientUserSessions : AbstractMultiMapIndexCreationTask<ClientSessionResults>
    {
        public All_ClientUserSessions()
        {
            AddMap<ClientSession>(clientSessions =>
                from c in clientSessions
                select new
                {
                    c.ClientId,
                    UserId = c.User.Id,
                    c.ConnectionCreated
                });

            TransformResults = (database, results) =>
                from result in results
                let user = database.Load<User>(result.UserId)
                select new
                {
                    result.ClientId,
                    result.UserId,
                    result.ConnectionCreated,
                    UserName = user.GetName(),
                    Avatar = user.Avatar.Id
                };

            Store(x => x.ClientId, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.UserName, FieldStorage.Yes);
            Store(x => x.Avatar, FieldStorage.Yes);
            Store(x => x.ConnectionCreated, FieldStorage.Yes);
        }
    }
}