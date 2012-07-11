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
    public class TeamsController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly ITeamViewModelBuilder _teamViewModelBuilder;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            ITeamViewModelBuilder teamViewModelBuilder,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IPermissionChecker permissionChecker,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(teamViewModelBuilder, "teamViewModelBuilder");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(permissionChecker, "permissionChecker");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _teamViewModelBuilder = teamViewModelBuilder;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
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
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            var viewModel = _activityViewModelBuilder.BuildGroupActivityList(teamId, activityInput, pagingInput);

            return RestfulResult(
                viewModel,
                "teams",
                "activity");
        }

        [HttpGet]
        public ActionResult Sightings(string id, PagingInput pagingInput)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Team = _teamViewModelBuilder.BuildTeam(teamId),
                Observations = _sightingViewModelBuilder.BuildGroupSightingList(teamId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "teams",
                "sightings");
        }

        [HttpGet]
        public ActionResult Posts(string id, PagingInput pagingInput)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Team = _teamViewModelBuilder.BuildTeam(teamId),
                Posts = _postViewModelBuilder.BuildGroupPostList(teamId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "teams",
                "posts");
        }

        [HttpGet]
        public ActionResult Members(string id, PagingInput pagingInput)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Team = _teamViewModelBuilder.BuildTeam(teamId),
                Members = _userViewModelBuilder.BuildGroupUserList(teamId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "teams",
                "members");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Team = _teamViewModelBuilder.BuildTeam(teamId)
            };

            return RestfulResult(
                viewModel,
                "teams",
                "index");
        }

        [HttpGet]
        public ActionResult List(string groupId, PagingInput pagingInput)
        {
            var viewModel = new
            {
                Teams = _teamViewModelBuilder.BuildGroupTeamList(groupId, true, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "teams",
                "list");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            // BUG: Need to check if approot/org has perms
            if (!_userContext.HasGroupPermission(PermissionNames.CreateTeam, string.Empty))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Team = _teamViewModelBuilder.BuildNewTeam(),
                Organisations = GetOrganisations(_userContext.GetAuthenticatedUserId())
            };

            return RestfulResult(
                viewModel,
                "teams",
                "create",
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateTeam, teamId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Team = _teamViewModelBuilder.BuildTeam(teamId),
                Organisations = GetOrganisations(_userContext.GetAuthenticatedUserId(), teamId)
            };

            return RestfulResult(
                viewModel,
                "teams",
                "update",
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteTeam, teamId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Team = _teamViewModelBuilder.BuildTeam(teamId)
            };

            return RestfulResult(
                viewModel,
                "teams",
                "delete",
                new Action<dynamic>(x => x.Model.Delete = true));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(string id)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            // TODO: Not sure what this permission check is actually checking???
            if (!_userContext.HasGroupPermission(PermissionNames.JoinTeam, teamId))
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
                    GroupId = teamId,
                    CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { RoleNames.TeamMember }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Leave(string id)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            // TODO: Not sure what this permission check is actually checking???
            if (!_userContext.HasGroupPermission(PermissionNames.LeaveTeam, teamId))
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
                    GroupId = teamId
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(TeamCreateInput createInput)
        {
            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamCreateCommand()
                    {
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Name = createInput.Name,
                        Description = createInput.Description,
                        Website = createInput.Website,
                        AvatarId = createInput.AvatarId,
                        OrganisationId = createInput.OrganisationId
                    }
                );

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(TeamUpdateInput updateInput)
        {
            string teamId = VerbosifyId<Team>(updateInput.Id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Project>(PermissionNames.UpdateProject, teamId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = teamId,
                    Name = updateInput.Name,
                    Description = updateInput.Description,
                    Website = updateInput.Website,
                    AvatarId = updateInput.AvatarId
                }
            );

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string teamId = VerbosifyId<Team>(id);

            if (!_permissionChecker.DoesExist<Team>(teamId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteProject, teamId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamDeleteCommand
                {
                    Id = teamId,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private object GetOrganisations(string userId, string teamId = null)
        {
            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.UserIds.Any(y => y == userId) && x.GroupType == "organisation")
                .ToList()
                .Select(x => new
                {
                    Text = x.Organisation.Name,
                    Value = x.Organisation.Id,
                    Selected = x.Organisation.DescendantGroups.Any(y => y.Id == teamId)
                });
        }

        #endregion
    }
}
