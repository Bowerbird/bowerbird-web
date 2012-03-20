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
using Bowerbird.Core.DomainModels.Sessions;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bowerbird.Core.Indexes
{
    public class All_UserSessions : AbstractMultiMapIndexCreationTask<All_UserSessions.Results>
    {
        public class Results
        {
            public string ClientId { get; set; }
            public string UserId { get; set; }
            public int Status { get; set; }
            public DateTime LatestActivity { get; set; }
        }

        public All_UserSessions()
        {
            AddMap<UserSession>(userSessions =>
                from c in userSessions
                select new
                {
                    c.ClientId,
                    UserId = c.User.Id,
                    c.LatestActivity,
                    c.Status
                });

            Store(x => x.ClientId, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.Status, FieldStorage.Yes);
            Store(x => x.LatestActivity, FieldStorage.Yes);
        }
    }
}