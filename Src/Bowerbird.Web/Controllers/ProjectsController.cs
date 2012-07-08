/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
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

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IProjectViewModelBuilder _projectViewModelBuilder;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly ITeamViewModelBuilder _teamViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IProjectViewModelBuilder projectViewModelBuilder,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            ITeamViewModelBuilder teamViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IPermissionChecker permissionChecker,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectViewModelBuilder, "projectViewModelBuilder");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(teamViewModelBuilder, "teamViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(permissionChecker, "permissionChecker");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _projectViewModelBuilder = projectViewModelBuilder;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _teamViewModelBuilder = teamViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _permissionChecker = permissionChecker;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Activity(string id, ActivityInput activityInput, PagingInput pagingInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            var viewModel = _activityViewModelBuilder.BuildGroupActivityList(projectId, activityInput, pagingInput);

            return RestfulResult(
                viewModel,
                "projects",
                "activity");
        }

        [HttpGet]
        public ActionResult Sightings(string id, PagingInput pagingInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Project = _projectViewModelBuilder.BuildProject(projectId),
                Observations = _sightingViewModelBuilder.BuildGroupSightingList(projectId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "projects",
                "sightings");
        }

        [HttpGet]
        public ActionResult Posts(string id, PagingInput pagingInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Project = _projectViewModelBuilder.BuildProject(projectId),
                Posts = _postViewModelBuilder.BuildGroupPostList(projectId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "projects",
                "posts");
        }

        [HttpGet]
        public ActionResult Members(string id, PagingInput pagingInput)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Project = _projectViewModelBuilder.BuildProject(projectId),
                Members = _userViewModelBuilder.BuildGroupUserList(projectId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "projects",
                "members");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Project = _projectViewModelBuilder.BuildProject(projectId)
            };

            return RestfulResult(
                viewModel,
                "projects",
                "index");
        }

        [HttpGet]
        public ActionResult List(string groupId, PagingInput pagingInput)
        {
            var viewModel = new
            {
                Projects = _projectViewModelBuilder.BuildGroupProjectList(groupId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "projects",
                "list");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            var viewModel = new
            {
                Project = _projectViewModelBuilder.BuildNewProject(),
                Teams = GetTeams(_userContext.GetAuthenticatedUserId())
            };

            return RestfulResult(
                viewModel,
                "projects",
                "create", 
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateProject, projectId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Project = _projectViewModelBuilder.BuildProject(projectId),
                Teams = GetTeams(_userContext.GetAuthenticatedUserId(), projectId)
            };

            return RestfulResult(
                viewModel,
                "projects",
                "update",
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, projectId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Project = _projectViewModelBuilder.BuildProject(projectId)
            };

            return RestfulResult(
                viewModel,
                "projects",
                "delete",
                new Action<dynamic>(x => x.Model.Delete = true));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            // TODO: Not sure what this permission check is actually checking???
            if (!_userContext.HasGroupPermission(PermissionNames.JoinProject, projectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new MemberCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId,
                    CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { RoleNames.ProjectMember }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Leave(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
            {
                return HttpNotFound();
            }

            // TODO: Not sure what this permission check is actually checking???
            if (!_userContext.HasGroupPermission(PermissionNames.LeaveProject, projectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new MemberDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = projectId
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

            _commandProcessor.Process(
                new ProjectCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Name = createInput.Name,
                    Description = createInput.Description,
                    Website = createInput.Website,
                    AvatarId = createInput.AvatarId,
                    TeamId = createInput.TeamId
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ProjectUpdateInput updateInput)
        {
            string projectId = VerbosifyId<Project>(updateInput.Id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
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

            _commandProcessor.Process(
                new ProjectUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = projectId,
                    Name = updateInput.Name,
                    Description = updateInput.Description,
                    Website = updateInput.Website,
                    AvatarId = updateInput.AvatarId
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string projectId = VerbosifyId<Project>(id);

            if (!_permissionChecker.DoesExist<Project>(projectId))
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

            _commandProcessor.Process(
                new ProjectDeleteCommand
                {
                    Id = projectId,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private IEnumerable GetTeams(string userId, string projectId = null)
        {
            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.UserIds.Any(y => y == userId) && x.GroupType == "team")
                .ToList()
                .Select(x => new
                        {
                            Text = x.Team.Name,
                            Value = x.Team.Id,
                            Selected = x.Team.Descendants.Any(y => y.Id == projectId)
                        });
        }

        #endregion
    }
}