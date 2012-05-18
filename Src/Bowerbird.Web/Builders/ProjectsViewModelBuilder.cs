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
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ProjectsViewModelBuilder(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Methods

        public object BuildProject(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .FirstOrDefault(x => x.Id == idInput.Id);

            return MakeProject(project);
        }

        public object BuildProjectList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.Id)
                .Where(x => x.GroupType == "project")
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeProject)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is User.Id
        /// </summary>
        public object BuildUserProjectList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var memberships = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == pagingInput.Id)
                .Include(x => x.Groups.Where(z => z.GroupType == "project").SelectMany(y => y.Id));

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Id.In(memberships.SelectMany(y => y.Groups.Select(z => z.Id))))
                .Customize(x => x.WaitForNonStaleResults())
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeProject)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is Team.Id
        /// </summary>
        public object BuildTeamProjectList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var teamProjects = _documentSession
                .Query<GroupAssociation>()
                .Include(x => x.ChildGroup.Id)
                .Where(x => x.ParentGroup.Id == pagingInput.Id && x.ChildGroup.GroupType == "project");

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Id.In(teamProjects.Select(y => y.ChildGroup.Id)))
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeProject)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is Project.Id
        /// </summary>
        public object BuildProjectUserList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<Member>()
                .Where(x => x.Group.Id == pagingInput.Id)
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.User.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeUser(x.User.Id))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        private object MakeUser(string userId)
        {
            return MakeUser(_documentSession.Load<User>(userId));
        }

        private object MakeUser(User user)
        {
            return new
            {
                Avatar = _avatarFactory.Make(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        private object MakeProject(All_Groups.Result project)
        {
            return new
            {
                project.Id,
                project.Project.Name,
                project.Project.Description,
                project.Project.Website,
                Avatar = _avatarFactory.Make(project.Project),
                project.GroupMemberCount
            };
        }

        #endregion
    }
}