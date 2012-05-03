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

            return _projectViewFactory.Make(_documentSession.Load<Project>(idInput.Id));
        }

        public object BuildIndex(StreamItemListInput streamItemListInput, StreamSortInput sortInput)
        {
            Check.RequireNotNull(streamItemListInput, "streamItemListInput");

            return new
            {
                Project = _projectViewFactory.Make(_documentSession.Load<Project>(streamItemListInput.GroupId)),
                StreamItems = _streamItemsViewModelBuilder.BuildStreamItems(streamItemListInput, sortInput),
                Observations = _observationsViewModelBuilder.BuildList(new ObservationListInput() { GroupId = streamItemListInput.GroupId }),
                Posts = _postViewModelBuilder.BuildList(new PostListInput(){ GroupId = streamItemListInput.GroupId}),
                Members = ProjectMembers(streamItemListInput.GroupId)
            };
        }

        public object BuildList(ProjectListInput listInput)
        {
            Check.RequireNotNull(listInput, "listInput");

            if(listInput.UserId != null)
            {
                return new { Projects = BuildProjectsForMember(listInput) };
            }

            if(listInput.TeamId != null)
            {
                return new { Projects = BuildProjectsForTeam(listInput) };
            }

            return new
            {
                Projects = BuildProjects(listInput)
            };
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

        private object BuildProjects(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.Id)
                .AsProjection<All_Groups.Result>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _projectViewFactory.Make(x.Project));

            return new
            {
                listInput.Page,
                listInput.PageSize,
                List = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object BuildProjectsForTeam(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var groupAssociations = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == listInput.TeamId);

            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .Where(x => x.Id.In(groupAssociations.Select(y => y.ParentGroupId)))
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.Id)
                .AsProjection<All_Groups.Result>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _projectViewFactory.Make(x.Project));

            return new
            {
                listInput.Page,
                listInput.PageSize,
                List = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object BuildProjectsForMember(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var memberships = _documentSession
                .Query<Member>()
                .Where(x => x.User.Id == listInput.UserId);

            var results = _documentSession
                .Query<Project>()
                .Where(x => x.Id.In(memberships.Select(y => y.Group.Id)))
                .Customize(x => x.Include<User>(y => y.Id == listInput.UserId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _projectViewFactory.Make(x));

            return new
            {
                User = _userViewFactory.Make(listInput.UserId),
                listInput.Page,
                listInput.PageSize,
                List = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}