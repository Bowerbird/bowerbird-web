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
using Bowerbird.Core.DomainModels;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System;

namespace Bowerbird.Core.Indexes
{
    public class All_Users : AbstractMultiMapIndexCreationTask<All_Users.Result>
    {
        public class Result
        {
            public string UserId { get; set; }
            public string[] MemberIds { get; set; }
            public object[] Memberships { get; set; }
        }

        public class ClientResult
        {
            public string UserId { get; set; }
            public string[] MemberIds { get; set; }
            public Membership[] Memberships { get; set; }
            public User User { get; set; }
            public IEnumerable<Member> Members { get; set; }
        }

        public class Membership
        {
            public string Id { get; set; }
            public DenormalisedGroupReference Group { get; set; }
            public Role[] Roles { get; set; }
        }

        public All_Users()
        {
            AddMap<Member>(members => from member in members
                                      let roles = member.Roles
                                      let permissions = roles.SelectMany(x => x.Permissions)
                                      select new
                                      {
                                          UserId = member.User.Id,
                                          MemberIds = new [] { member.Id },
                                          Memberships = new
                                          {
                                              member.Id,
                                              @group = new
                                              {
                                                  member.Group.Id,
                                                  member.Group.GroupType
                                              },
                                              roles = from role in member.Roles
                                                      select new
                                                      {
                                                          role.Id,
                                                          role.Name,
                                                          role.Description,
                                                          permissions = from permission in role.Permissions
                                                                        select new
                                                                        {
                                                                            permission.Id,
                                                                            permission.Name,
                                                                            permission.Description
                                                                        }
                                                      }
                                          }
                                      });

            Reduce = results => from result in results
                                group result by result.UserId
                                    into g
                                    select new
                                    {
                                        UserId = g.Key,
                                        MemberIds = g.SelectMany(x => x.MemberIds),
                                        Memberships = g.SelectMany(x => x.Memberships)
                                    };

            TransformResults = (database, results) =>
                                from result in results
                                let user = database.Load<User>(result.UserId)
                                let members = database.Load<Member>(result.MemberIds)
                                select new
                                {
                                    result.UserId,
                                    result.MemberIds,
                                    result.Memberships,
                                    User = user,
                                    Members = members
                                };

            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.MemberIds, FieldStorage.Yes);
            Store(x => x.Memberships, FieldStorage.Yes);
        }
    }
}