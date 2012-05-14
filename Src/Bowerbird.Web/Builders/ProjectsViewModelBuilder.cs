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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Builders
{
    public class ProjectsViewModelBuilder : IProjectsViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IProjectViewFactory _projectViewFactory;

        #endregion

        #region Constructors

        public ProjectsViewModelBuilder(
            IDocumentSession documentSession,
            IProjectViewFactory projectViewFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(projectViewFactory, "projectViewFactory");

            _documentSession = documentSession;
            _projectViewFactory = projectViewFactory;
        }

        #endregion

        #region Methods

        public object BuildProject(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Id == idInput.Id)
                .FirstOrDefault();

            return _projectViewFactory.Make(project);
        }

        public object BuildProjectList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.Id)
                .Where(x => x.GroupType == "project")
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _projectViewFactory.Make(x));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                List = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildUserProjectList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var memberships = _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.UserId == pagingInput.Id && x.GroupId.Contains("projects/"))
                .Select(x => x.GroupId)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList();

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .Where(x => x.Id.In(memberships))
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.Id)
                .AsProjection<All_Groups.Result>()
                .ToList()
                .Select(x => _projectViewFactory.Make(x));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                List = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        /// <summary>
        /// Get all the projects in a given team
        /// </summary>
        public object BuildTeamProjectList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            // team children are projects so get projects for team.
            var teamProjects = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == pagingInput.Id)
                .Include(x => x.ChildGroupId)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => x.ChildGroupId);

            // load the actual projects
            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .Where(x => x.Project.Id.In(teamProjects))
                .Customize(x => x.WaitForNonStaleResults())
                .AsProjection<All_Groups.Result>()
                .ToList()
                .Select(x => _projectViewFactory.Make(x));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                List = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        /// <summary>
        /// Get all the members in a given project
        /// </summary>
        public object BuildProjectMemberList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Member>()
                .Where(x => x.Group.Id == pagingInput.Id)
                .Customize(x => x.WaitForNonStaleResults())
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList();

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                List = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}