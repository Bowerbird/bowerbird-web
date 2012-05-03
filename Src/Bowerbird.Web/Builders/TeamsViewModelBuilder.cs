/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

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

namespace Bowerbird.Web.Builders
{
    public class TeamsViewModelBuilder : ITeamsViewModelBuilder
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IUsersGroupsQuery _usersGroupsQuery;
        private readonly IOrganisationViewFactory _organisationViewFactory;
        private readonly IDocumentSession _documentSession;
        private readonly ITeamViewFactory _teamViewFactory;

        #endregion

        #region Constructors

        public TeamsViewModelBuilder(
            IUserContext userContext,
            IUsersGroupsQuery usersGroupsQuery,
            IOrganisationViewFactory organisationViewFactory,
            IDocumentSession documentSession,
            ITeamViewFactory teamViewFactory
        )
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(usersGroupsQuery, "usersGroupsQuery");
            Check.RequireNotNull(organisationViewFactory, "organisationViewFactory");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(teamViewFactory, "teamViewFactory");

            _userContext = userContext;
            _usersGroupsQuery = usersGroupsQuery;
            _organisationViewFactory = organisationViewFactory;
            _documentSession = documentSession;
            _teamViewFactory = teamViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildItem(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return new {
                Team = _teamViewFactory.Make(_documentSession.Load<Team>(idInput.Id))
            };
        }

        public object BuildList(PagingInput pagingInput)
        {
            if(listInput.OrganisationId != null)
            {
                return BuildTeamsForOrganisation(listInput);
            }

            if(listInput.HasAddProjectPermission)
            {
                return BuildTeamsWhereUserHasAddProjectPermission();
            }

            return BuildTeams(listInput);
        }

        private object BuildTeams(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var teams = _documentSession
                .Query<Team>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "team" && x.Id.In(teams.Select(y => y.Id)))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _teamViewFactory.Make(x.Team));

            return new
            {
                listInput.Page,
                listInput.PageSize,
                Teams = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object BuildTeamsForOrganisation(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var teams = _documentSession
                .Query<GroupAssociation>()
                .Include(x => x.ChildGroupId)
                .Where(x => x.ParentGroupId == listInput.OrganisationId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "team" && x.Id.In(teams.Select(y => y.ParentGroupId)))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _teamViewFactory.Make(x.Team));

            return new
            {
                listInput.Page,
                listInput.PageSize,
                Teams = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object BuildTeamsWhereUserHasAddProjectPermission()
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
                .Select(x => _teamViewFactory.Make(x.Team));
        }

        #endregion
    }
}