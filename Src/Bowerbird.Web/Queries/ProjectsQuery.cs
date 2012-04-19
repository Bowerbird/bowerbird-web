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
using Bowerbird.Core.Paging;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Queries
{
    public class ProjectsQuery : IProjectsQuery
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IUsersGroupsQuery _usersGroupsQuery;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ProjectsQuery(
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

        #region Methods

        public ProjectIndex MakeProjectIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession.Load<Project>(idInput.Id);

            return new ProjectIndex()
            {
                Project = project,
                Avatar = _avatarFactory.GetAvatar(project)
            };
        }

        public ProjectList MakeProjectList(ProjectListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Project>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(project => new ProjectView()
                {
                    Id = project.Id,
                    Description = project.Description,
                    Name = project.Name,
                    Website = project.Website,
                    Avatar = _avatarFactory.GetAvatar(project)
                });

            var projects = new ProjectList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Projects = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };

            return projects;
        }

        public ProjectList MakeProjectListByMembership(ProjectListInput listInput)
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
                .Select(project => new ProjectView()
                {
                    Id = project.Id,
                    Description = project.Description,
                    Name = project.Name,
                    Website = project.Website,
                    Avatar = _avatarFactory.GetAvatar(project)
                });

            return new ProjectList
            {
                User = listInput.UserId != null ? _documentSession.Load<User>(listInput.UserId) : null,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Projects = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}