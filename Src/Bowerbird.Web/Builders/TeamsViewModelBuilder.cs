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

        public object BuildTeam(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _teamViewFactory.Make(_documentSession.Load<Team>(idInput.Id));
        }

        public object BuildTeamList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            //var teams = _documentSession
            //    .Query<Team>()
            //    .Statistics(out stats)
            //    .Skip(pagingInput.Page)
            //    .Take(pagingInput.PageSize)
            //    .ToList()
            //    .Select(x => x.Id);

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Customize(x => x.WaitForNonStaleResults())
                .Where(x => x.GroupType == "team")
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _teamViewFactory.Make(x.Team));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Teams = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildUserTeamList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var memberships = _documentSession
               .Query<All_UserMemberships.Result, All_UserMemberships>()
               .Where(x => x.UserId == pagingInput.Id && x.GroupId.Contains("teams/"))
               .Include(x => x.GroupId)
               .Select(x => x.GroupId)
               .Statistics(out stats)
               .Skip(pagingInput.Page)
               .Take(pagingInput.PageSize)
               .ToList();

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "team" && x.Id.In(memberships))
                .ToList()
                .Select(x => _teamViewFactory.Make(x.Team));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Teams = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildOrganisationTeamList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var teams = _documentSession
                .Query<GroupAssociation>()
                .Include(x => x.ChildGroupId)
                .Where(x => x.ParentGroupId == pagingInput.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList();

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "team" && x.Id.In(teams.Select(y => y.ParentGroupId)))
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _teamViewFactory.Make(x.Team));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Teams = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}