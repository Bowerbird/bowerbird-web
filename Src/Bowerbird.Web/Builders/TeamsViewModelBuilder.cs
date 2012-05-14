/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
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

        private readonly IDocumentSession _documentSession;
        private readonly ITeamViewFactory _teamViewFactory;

        #endregion

        #region Constructors

        public TeamsViewModelBuilder(
            IDocumentSession documentSession,
            ITeamViewFactory teamViewFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(teamViewFactory, "teamViewFactory");

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

            var team = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Id == idInput.Id)
                .FirstOrDefault();

            return _teamViewFactory.Make(team);
        }

        public object BuildTeamList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Customize(x => x.WaitForNonStaleResults())
                .Where(x => x.GroupType == "team")
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _teamViewFactory.Make(x));

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
               .Query<All_Users.Result, All_Users>()
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
                .Select(x => _teamViewFactory.Make(x));

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
                .Select(x => _teamViewFactory.Make(x));

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