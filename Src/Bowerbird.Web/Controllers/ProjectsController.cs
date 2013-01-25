/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;
using System;
using System.Linq;
using System.Collections;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Controllers
{
    public class ProjectsController : ControllerBase
    {
        #region Fields

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IProjectViewModelBuilder _projectViewModelBuilder;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IPermissionManager _permissionManager;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectsController(
            IMessageBus messageBus,
            IUserContext userContext,
            IProjectViewModelBuilder projectViewModelBuilder,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IPermissionManager permissionManager,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectViewModelBuilder, "projectViewModelBuilder");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(documentSession, "documentSession");

            _messageBus = messageBus;
            _userContext = userContext;
            _projectViewModelBuilder = projectViewModelBuilder;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _permissionManager = permissionManager;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Sightings(string id, SightingsQueryInput queryInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (queryInput.View.ToLower() == "thumbnails")
            {
                queryInput.PageSize = 15;
            }

            if (queryInput.View.ToLower() == "details")
            {
                queryInput.PageSize = 10;
            }

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "newest";
            }

            dynamic project = _projectViewModelBuilder.BuildProject(projectId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelBuilder.BuildProject(projectId);
            viewModel.Sightings = _sightingViewModelBuilder.BuildGroupSightingList(projectId, queryInput);
            viewModel.MemberCountDescription = "Member" + (project.MemberCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (project.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (project.PostCount == 1 ? string.Empty : "s");
            viewModel.Query = new
            {
                Id = projectId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.View,
                IsThumbnailsView = queryInput.View == "thumbnails",
                IsDetailsView = queryInput.View == "details",
                IsMapView = queryInput.View == "map"
            };
            viewModel.ShowSightings = true;

            return RestfulResult(
                viewModel,
                "projects",
                "sightings");
        }

        [HttpGet]
        public ActionResult Posts(string id, PostsQueryInput queryInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            queryInput.PageSize = 10;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "newest";
            }

            dynamic project = _projectViewModelBuilder.BuildProject(projectId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelBuilder.BuildProject(projectId);
            viewModel.Posts = _postViewModelBuilder.BuildGroupPostList(projectId, queryInput);
            viewModel.MemberCountDescription = "Member" + (project.MemberCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (project.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (project.PostCount == 1 ? string.Empty : "s");
            viewModel.Query = new
            {
                Id = projectId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort
            };
            viewModel.ShowPosts = true;

            return RestfulResult(
                viewModel,
                "projects",
                "posts");
        }

        [HttpGet]
        public ActionResult Members(string id, UsersQueryInput queryInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            queryInput.PageSize = 15;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "a-z";
            }

            dynamic project = _projectViewModelBuilder.BuildProject(projectId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelBuilder.BuildProject(projectId);
            viewModel.Members = _userViewModelBuilder.BuildGroupUserList(projectId, queryInput);
            viewModel.Query = new
            {
                Id = projectId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort
            };
            viewModel.ShowMembers = true;
            viewModel.IsMember = _userContext.HasGroupPermission<Project>(PermissionNames.CreateObservation, projectId);
            viewModel.MemberCountDescription = "Member" + (project.MemberCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (project.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (project.PostCount == 1 ? string.Empty : "s");

            return RestfulResult(
                viewModel,
                "projects",
                "members");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            dynamic project = _projectViewModelBuilder.BuildProject(projectId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelBuilder.BuildProject(projectId);
            viewModel.ShowAbout = true;
            viewModel.IsMember = _userContext.HasGroupPermission<Project>(PermissionNames.CreateObservation, projectId);
            viewModel.MemberCountDescription = "Member" + (project.MemberCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (project.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (project.PostCount == 1 ? string.Empty : "s");
            viewModel.ProjectAdministrators = _userViewModelBuilder.BuildGroupUserList(projectId, "roles/" + RoleNames.ProjectAdministrator);
            viewModel.ActivityTimeseries = CreateActivityTimeseries(projectId);

            return RestfulResult(
                viewModel,
                "projects",
                "about");
        }

        [HttpGet]
        public ActionResult Index(string id, ActivityInput activityInput, PagingInput pagingInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            dynamic project = _projectViewModelBuilder.BuildProject(projectId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = project;
            viewModel.Activities = _activityViewModelBuilder.BuildGroupActivityList(projectId, activityInput, pagingInput);
            viewModel.IsMember = _userContext.HasGroupPermission<Project>(PermissionNames.CreateObservation, projectId);
            viewModel.MemberCountDescription = "Member" + (project.MemberCount == 1 ? string.Empty : "s");
            viewModel.SightingCountDescription = "Sighting" + (project.SightingCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (project.PostCount == 1 ? string.Empty : "s");
            viewModel.ShowActivities = true;

            return RestfulResult(
                viewModel,
                "projects",
                "index");
        }

        [HttpGet]
        public ActionResult List(ProjectsQueryInput queryInput)
        {
            queryInput.PageSize = 15;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "popular" &&
                queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "popular";
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Projects = _projectViewModelBuilder.BuildProjectList(queryInput);
            viewModel.Query = new
            {
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort
            };

            if (_userContext.IsUserAuthenticated())
            {
                var user = _documentSession
                    .Query<All_Users.Result, All_Users>()
                    .AsProjection<All_Users.Result>()
                    .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                    .Single();

                viewModel.ShowProjectExploreWelcome = user.User.CallsToAction.Contains("project-explore-welcome");
            }

            return RestfulResult(
                viewModel,
                "projects",
                "list");    
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelBuilder.BuildCreateProject();
            viewModel.Create = true;

            return RestfulResult(
                viewModel,
                "projects",
                "create");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateProject, projectId))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelBuilder.BuildUpdateProject(projectId);
            viewModel.Update = true;

            return RestfulResult(
                viewModel,
                "projects",
                "update");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, projectId))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Project = _projectViewModelBuilder.BuildProject(projectId);
            viewModel.Delete = true;

            return RestfulResult(
                viewModel,
                "projects",
                "delete");
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult UpdateMember(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed(); 
            }

            _messageBus.Send(
                new MemberUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId,
                    ModifiedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { "roles/projectmember" }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMember(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            //// TODO: Not sure what this permission check is actually checking???
            //if (!_userContext.HasGroupPermission(PermissionNames.LeaveProject, projectId))
            //{
            //    return HttpUnauthorized();
            //}

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new MemberDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId,
                    ModifiedByUserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new ProjectCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Name = createInput.Name,
                    Description = createInput.Description,
                    Website = createInput.Website,
                    AvatarId = createInput.AvatarId,
                    BackgroundId = createInput.BackgroundId
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ProjectUpdateInput updateInput)
        {
            string projectId = VerbosifyId<Project>(updateInput.Id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Project>(PermissionNames.UpdateProject, projectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new ProjectUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = projectId,
                    Name = updateInput.Name,
                    Description = updateInput.Description,
                    Website = updateInput.Website,
                    AvatarId = updateInput.AvatarId,
                    BackgroundId =  updateInput.BackgroundId
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionManager.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, projectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new ProjectDeleteCommand
                {
                    Id = projectId,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private dynamic CreateActivityTimeseries(string projectId)
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)).Date;
            var toDate = DateTime.UtcNow.Date;

            var result = _documentSession
                .Advanced
                .LuceneQuery<All_Contributions.Result, All_Contributions>()
                .SelectFields<All_Contributions.Result>("ParentContributionId", "SubContributionId", "ParentContributionType", "SubContributionType", "CreatedDateTime")
                .WhereGreaterThan(x => x.CreatedDateTime, fromDate)
                .AndAlso()
                .WhereIn("GroupIds", new [] { projectId })
                .AndAlso()
                .OpenSubclause()
                .WhereIn("ParentContributionType", new[] { "observation", "record", "post" })
                .OrElse()
                .WhereIn("SubContributionType", new[] { "note", "comment" })
                .CloseSubclause()
                .ToList();

            var contributions = result.Select(x => new
            {
                x.ParentContributionId,
                x.SubContributionId,
                x.ParentContributionType,
                x.SubContributionType,
                x.CreatedDateTime
            })
            .GroupBy(x => x.CreatedDateTime.Date);

            var timeseries = new List<dynamic>();

            for (DateTime dateItem = fromDate; dateItem <= toDate; dateItem = dateItem.AddDays(1))
            {
                string createdDateFormat;

                if (dateItem == fromDate ||
                    dateItem.Day == 1)
                {
                    createdDateFormat = "d MMM";
                }
                else
                {
                    createdDateFormat = "%d";
                }

                if (contributions.Any(x => x.Key.Date == dateItem.Date))
                {
                    timeseries.Add(contributions
                        .Where(x => x.Key.Date == dateItem.Date)
                        .Select(x => new
                        {
                            CreatedDate = dateItem.ToString(createdDateFormat),
                            SightingCount = x.Count(y => y.ParentContributionType == "observation" || y.ParentContributionType == "record"),
                            NoteCount = x.Count(y => y.SubContributionType == "note"),
                            PostCount = x.Count(y => y.ParentContributionType == "post"),
                            CommentCount = x.Count(y => y.SubContributionType == "comment")
                        }
                        ).First());
                }
                else
                {
                    timeseries.Add(new
                    {
                        CreatedDate = dateItem.ToString(createdDateFormat),
                        SightingCount = 0,
                        NoteCount = 0,
                        PostCount = 0,
                        CommentCount = 0
                    });
                }
            }
            return timeseries;
        }

        #endregion
    }
}