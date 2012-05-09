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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.Queries
{
    public class UsersGroupsQuery : IUsersGroupsQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UsersGroupsQuery(
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods
				
        public IEnumerable<string> GetUsersGroupsHavingPermission(string userId, string permissionId)
        {
            var memberships = _documentSession
                .Query<Member>()
                .Where(x => x.User.Id == userId)
                .ToList();

            var groups = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(g =>
                    g.Id.In(
                        memberships
                        .Where(y => y.Roles.Any(z => z.PermissionIds.Contains("permissions/" + permissionId)))
                        .Select(x => x.Group.Id)
                    ))
                .ToList();

            return groups.Select(x => x.Id);
        }

        public int GetGroupMemberCount(string groupId)
        {
            return _documentSession
                .Query<All_Users.Result, All_Groups>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.GroupId == groupId)
                .Count();
        }

        #endregion
    }
}