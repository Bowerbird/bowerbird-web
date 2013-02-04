/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.DomainModelFactories;
using Bowerbird.Core.ViewModelFactories;
using System.Collections.Generic;

namespace Bowerbird.Core.Queries
{
    public class ProjectViewModelQuery : IProjectViewModelQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ProjectViewModelQuery(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _userContext = userContext;
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
                BackgroundId = string.Empty,
                Categories = new string[] { }
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
                project.Background,
                project.Categories
            };
        }

        public object BuildProject(string projectId)
        {
            Check.RequireNotNullOrWhitespace(projectId, "projectId");

            var project = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .First(x => x.GroupId == projectId);

            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            return _groupViewFactory.Make(project.Group, authenticatedUser, true, project.SightingCount, project.UserCount, project.PostCount);
        }

        public object BuildProjectList(ProjectsQueryInput projectsQueryInput)
        {
            Check.RequireNotNull(projectsQueryInput, "projectsQueryInput");

            return ExecuteQuery(projectsQueryInput);
        }

        private object ExecuteQuery(ProjectsQueryInput projectsQueryInput)
        {
            RavenQueryStatistics stats;
            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Groups.Result, All_Groups>()
                .Statistics(out stats)
                .SelectFields<All_Groups.Result>("GroupType", "GroupId", "CreatedDateTime", "UserCount", "SightingCount", "PostCount", "VoteCount", "LatestObservationIds", "LatestObservations", "SortName")
                .WhereEquals("GroupType", "project");

            if (!string.IsNullOrWhiteSpace(projectsQueryInput.Category))
            {
                query = query
                    .AndAlso()
                    .WhereIn("Categories", new [] { projectsQueryInput.Category });
            }

            if (!string.IsNullOrWhiteSpace(projectsQueryInput.Query))
            {
                var field = "AllFields";

                if (projectsQueryInput.Field.ToLower() == "name")
                {
                    field = "Name";
                }
                if (projectsQueryInput.Field.ToLower() == "description")
                {
                    field = "Description";
                }

                query = query
                    .AndAlso()
                    .Search(field, projectsQueryInput.Query);
            }

            switch (projectsQueryInput.Sort.ToLower())
            {
                case "a-z":
                    query = query.AddOrder(x => x.SortName, false);
                    break;
                case "z-a":
                    query = query.AddOrder(x => x.SortName, true);
                    break;
                case "newest":
                    query = query.AddOrder(x => x.CreatedDateTime, true);
                    break;
                case "oldest":
                    query = query.AddOrder(x => x.CreatedDateTime, false);
                    break;
                default:
                case "popular":
                    query = query.AddOrder(x => x.UserCount, true);
                    break;
            }

            return query
                .Skip(projectsQueryInput.GetSkipIndex())
                .Take(projectsQueryInput.GetPageSize())
                .ToList()
                .Select(x => _groupViewFactory.Make(x.Group, authenticatedUser, true, x.SightingCount, x.UserCount, x.PostCount, x.LatestObservations != null ? x.LatestObservations.Take(4) : null))
                .ToPagedList(
                    projectsQueryInput.GetPage(),
                    projectsQueryInput.GetPageSize(),
                    stats.TotalResults
                );
        }

        #endregion
    }
}