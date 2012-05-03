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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System;

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

        public object BuildItem(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _projectViewFactory.Make(_documentSession.Load<Project>(Convert.ToInt32(idInput.Id)));
        }

        public object BuildIndex(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            return new
            {
                Project = _projectViewFactory.Make(_documentSession.Load<Project>(pagingInput.Id)),
                StreamItems = _streamItemsViewModelBuilder.BuildGroupStreamItems(pagingInput),
                Observations = _observationsViewModelBuilder.BuildList(pagingInput),
                Posts = _postViewModelBuilder.BuildList(pagingInput),
                Members = ProjectMembers(pagingInput.Id),
                PrerenderedView = "projects" // HACK: Need to rethink this
            };
        }

        public object BuildList(PagingInput pagingInput)
        {
            //Check.RequireNotNull(pagingInput, "pagingInput");

            //if (pagingInput.UserId != null)
            //{
            //    return new { Projects = BuildProjectsForMember(pagingInput) };
            //}

            //if (pagingInput.TeamId != null)
            //{
            //    return new { Projects = BuildProjectsForTeam(pagingInput) };
            //}

            //return new
            //{
            //    Projects = BuildProjects(pagingInput)
            //};
            throw new System.NotImplementedException();
        }

        private object ProjectMembers(string groupId)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_UserMemberships.Result, All_UserMemberships>()
                .AsProjection<All_UserMemberships.Result>()
                .Where(x => x.GroupId == groupId)
                .Include(x => x.UserId)
                .Statistics(out stats)
                .Skip(Default.PageStart)
                .Take(Default.PageSize)
                .ToList()
                .Select(x => _userViewFactory.Make(x.UserId));

            return new
            {
                Default.PageStart,
                Default.PageSize,
                List = results.ToPagedList(
                    Default.PageStart,
                    Default.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object BuildProjects(PagingInput pagingInput)
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

        private object BuildProjectsForTeam(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groupAssociations = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == pagingInput.Id);

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .Where(x => x.Id.In(groupAssociations.Select(y => y.ParentGroupId)))
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

        private object BuildProjectsForMember(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var memberships = _documentSession
                .Query<Member>()
                .Where(x => x.User.Id == pagingInput.Id);

            var results = _documentSession
                .Query<Project>()
                .Where(x => x.Id.In(memberships.Select(y => y.Group.Id)))
                .Customize(x => x.Include<User>(y => y.Id == pagingInput.Id))
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _projectViewFactory.Make(x));

            return new
            {
                User = _userViewFactory.Make(pagingInput.Id),
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