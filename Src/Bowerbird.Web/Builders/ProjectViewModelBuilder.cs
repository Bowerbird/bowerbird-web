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
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;
using Bowerbird.Web.Factories;
using System.Collections.Generic;

namespace Bowerbird.Web.Builders
{
    public class ProjectViewModelBuilder : IProjectViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public ProjectViewModelBuilder(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
        }

        #endregion

        #region Methods

        public object BuildCreateProject()
        {
            return new
            {
                Name = string.Empty,
                Description = string.Empty,
                Website = string.Empty,
                Avatar = _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Project),
                Background = _mediaResourceFactory.MakeDefaultBackgroundImage("project"),
                AvatarId = string.Empty,
                BackgroundId = string.Empty
            };
        }

        public object BuildUpdateProject(string projectId)
        {
            var project = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .First(x => x.GroupId == projectId)
                .Project;

            return new
            {
                project.Id,
                project.Name,
                project.Description,
                project.Website,
                AvatarId = project.Avatar.Id,
                BackgroundId = project.Background.Id,
                project.Avatar,
                project.Background
            };
        }

        public dynamic BuildProject(string projectId)
        {
            Check.RequireNotNullOrWhitespace(projectId, "projectId");

            var project = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .First(x => x.GroupId == projectId);

            return _groupViewFactory.Make(project, true);
        }

        public object BuildProjectList(ProjectsQueryInput projectsQueryInput)
        {
            Check.RequireNotNull(projectsQueryInput, "projectsQueryInput");

            var query = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "project");

            return ExecuteQuery(projectsQueryInput.Sort, projectsQueryInput, query);
        }

        public object BuildUserProjectList(string userId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "project" && x.UserIds.Any(y => y == userId));

            return ExecuteQuery("a-z", pagingInput, query);
        }

        private object ExecuteQuery(string sort, PagingInput pagingInput, IRavenQueryable<All_Groups.Result> query)
        {
            switch (sort.ToLower())
            {
                default:
                case "newest":
                    query = query.OrderByDescending(x => x.CreatedDateTime);
                    break;
                case "a-z":
                    query = query.OrderBy(x => x.Name);
                    break;
                case "z-a":
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case "oldest":
                    query = query.OrderBy(x => x.CreatedDateTime);
                    break;
            }

            RavenQueryStatistics stats;

            return query.Skip(pagingInput.GetSkipIndex())
                .Statistics(out stats)
                .Take(pagingInput.GetPageSize())
                .ToList()
                .Select(x => _groupViewFactory.Make(x, true))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}