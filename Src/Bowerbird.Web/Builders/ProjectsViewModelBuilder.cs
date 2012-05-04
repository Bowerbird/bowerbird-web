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
        private readonly IUserViewFactory _userViewFactory;
        private readonly IStreamItemsViewModelBuilder _streamItemsViewModelBuilder;
        private readonly IPostsViewModelBuilder _postViewModelBuilder;
        private readonly IObservationsViewModelBuilder _observationsViewModelBuilder;

        #endregion

        #region Constructors

        public ProjectsViewModelBuilder(
            IDocumentSession documentSession,
            IProjectViewFactory projectViewFactory,
            IUserViewFactory userViewFactory,
            IStreamItemsViewModelBuilder streamItemsViewModelBuilder,
            IPostsViewModelBuilder postViewModelBuilder,
            IObservationsViewModelBuilder observationsViewModelBuilder
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(projectViewFactory, "projectViewFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(streamItemsViewModelBuilder, "streamItemsViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postsViewModelBuilder");
            Check.RequireNotNull(observationsViewModelBuilder, "observationsViewModelBuilder");

            _documentSession = documentSession;
            _projectViewFactory = projectViewFactory;
            _userViewFactory = userViewFactory;
            _streamItemsViewModelBuilder = streamItemsViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
            _observationsViewModelBuilder = observationsViewModelBuilder;
        }

        #endregion

        #region Methods

        public object BuildProject(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _projectViewFactory.Make(_documentSession.Load<Project>(idInput.Id));
        }

        public object BuildProjectList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.Id)
                .AsProjection<All_Groups.Result>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _projectViewFactory.Make(x.Project));

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
                //.ToList()
                .Select(x => x.ChildGroupId);

            // load the actual projects
            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .Where(x => x.Project.Id.In(teamProjects))
                .Customize(x => x.WaitForNonStaleResults())
                .AsProjection<All_Groups.Result>()
                .ToList()
                .Select(x => _projectViewFactory.Make(x.Project));

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