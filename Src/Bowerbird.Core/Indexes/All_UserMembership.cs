/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Bowerbird.Core.Indexes
{
    public class All_UserMemberships : AbstractMultiMapIndexCreationTask<All_UserMemberships.Result>
    {
        public class Result
        {
            public string Id { get; set; }
            public string UserId { get; set; }
            public string GroupId { get; set; }
            public object RoleIds { get; set; }
            public object PermissionIds { get; set; }
            public Member Member { get; set; }
        }

        public All_UserMemberships()
        {
            AddMap<Member>(members => from member in members 
                                        select new
                                        {
                                            member.Id,
                                            UserId = member.User.Id,
                                            GroupId = member.Group.Id
                                        }); 
            
            TransformResults = (database, results) =>
                                from result in results
                                let member = database.Load<Member>(result.Id)
                                let roleids = member.Roles.Select(x => x.Id).ToArray()
                                let permissionIds = member.Roles.Select(x => x.PermissionIds).ToArray()
                                select new
                                {
                                    result.Id,
                                    result.UserId,
                                    result.GroupId,
                                    RoleIds = roleids.ToArray(),
                                    PermissionIds = permissionIds.ToArray(),
                                    Member = member
                                };

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.UserId, FieldStorage.Yes);
            Store(x => x.GroupId, FieldStorage.Yes);
            Store(x => x.RoleIds, FieldStorage.Yes);
            Store(x => x.PermissionIds, FieldStorage.Yes);
        }
    }
}