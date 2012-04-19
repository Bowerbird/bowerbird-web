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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Web.Queries
{
    public class TeamsQuery : ITeamsQuery
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IUsersGroupsQuery _usersGroupsQuery;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public TeamsQuery(
            IUserContext userContext,
            IDocumentSession documentSession,
            IUsersGroupsQuery usersGroupsQuery,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(usersGroupsQuery, "usersGroupsQuery");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _userContext = userContext;
            _documentSession = documentSession;
            _usersGroupsQuery = usersGroupsQuery;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public List<TeamView> TeamsHavingAddProjectPermission()
        {
            var loggedInUserId = _userContext.GetAuthenticatedUserId();

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x =>
                    x.Id.In(_usersGroupsQuery.GetUsersGroupsHavingPermission(loggedInUserId, "createproject")) &&
                    x.GroupType == "team"
                )
                .ToList()
                .Select(team => new TeamView()
                {
                    Id = team.Id,
                    Description = team.Team.Description,
                    Name = team.Team.Name,
                    Website = team.Team.Website,
                    Avatar = _avatarFactory.GetAvatar(team.Team)
                })
                .ToList();
        }

        public TeamIndex MakeTeamIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var team = _documentSession.Load<Team>(idInput.Id);

            var groupAssociations = _documentSession
                .Query<GroupAssociation>()
                .Include(x => x.ChildGroupId)
                .Where(x => x.ParentGroupId == team.Id);

            return new TeamIndex()
            {
                Team = team,
                Projects = _documentSession.Load<Project>(groupAssociations.Select(x => x.ChildGroupId)),
                Avatar = _avatarFactory.GetAvatar(team)
            };
        }

        public TeamList MakeTeamList(TeamListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Team>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(team => new TeamView()
                {
                    Id = team.Id,
                    Description = team.Description,
                    Name = team.Name,
                    Website = team.Website,
                    Avatar = _avatarFactory.GetAvatar(team)
                });

            return new TeamList()
            {
                Organisation = listInput.OrganisationId != null ? _documentSession.Load<Organisation>(listInput.OrganisationId) : null,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Teams = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}