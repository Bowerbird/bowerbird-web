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
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class TeamController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IUsersGroupsHavingPermissionQuery _usersGroupsHavingPermissionQuery;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public TeamController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IUsersGroupsHavingPermissionQuery usersGroupsHavingPermissionQuery,
            IAvatarFactory avatarFactory
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(usersGroupsHavingPermissionQuery, "usersGroupsHavingPermissionQuery");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _usersGroupsHavingPermissionQuery = usersGroupsHavingPermissionQuery;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(MakeTeamIndex(idInput));
                }

                return View(MakeTeamIndex(idInput));
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List(TeamListInput listInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (listInput.HasAddProjectPermission)
                {
                    return new JsonNetResult(GetGroupsHavingAddProjectPermission());
                }

                if (Request.IsAjaxRequest())
                {
                    return new JsonNetResult(MakeTeamList(listInput));
                }
            }

            return View();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(TeamCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateTeam, createInput.Organisation ?? Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(TeamUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission<Team>(PermissionNames.UpdateTeam, updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeUpdateCommand(updateInput));

            return Json("Success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasGroupPermission<Team>(PermissionNames.DeleteTeam, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult CreateProject(ProjectCreateInput projectCreateInput, TeamProjectCreateInput teamProjectCreateInput)
        {
            if(_userContext.HasGroupPermission(teamProjectCreateInput.TeamId, PermissionNames.CreateProject))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeTeamProjectCreateCommand(projectCreateInput, teamProjectCreateInput));

            return Json("success");
        }

        private TeamCreateCommand MakeCreateCommand(TeamCreateInput createInput)
        {
            return new TeamCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId(),
                OrganisationId = createInput.Organisation
            };
        }

        private TeamDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new TeamDeleteCommand()
            {
                Id = deleteInput.Id,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private TeamUpdateCommand MakeUpdateCommand(TeamUpdateInput updateInput)
        {
            return new TeamUpdateCommand()
            {
                Description = updateInput.Description,
                Name = updateInput.Name,
                UserId = _userContext.GetAuthenticatedUserId(),
                AvatarId = updateInput.AvatarId
            };
        }

        private TeamProjectCreateCommand MakeTeamProjectCreateCommand(ProjectCreateInput projectCreateInput, TeamProjectCreateInput teamProjectCreateInput)
        {
            return new TeamProjectCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Name = projectCreateInput.Name,
                Description = projectCreateInput.Description,
                Administrators = teamProjectCreateInput.Administrators,
                Members = teamProjectCreateInput.Members,
                TeamId = teamProjectCreateInput.TeamId
            };
        }

        private TeamIndex MakeTeamIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var team = _documentSession.Load<Team>(idInput.Id);

            var groupAssociations = _documentSession
                .Query<GroupAssociation>()
                .Include(x => x.ChildGroupId)
                .Where(x => x.ParentGroupId == team.Id);

            return new TeamIndex()
            {
                Team = team,
                Projects = _documentSession.Load<Project>(groupAssociations.Select(x => x.ChildGroupId)),
                Avatar = _avatarFactory.GetAvatar(team)
            };
        }

        private TeamList MakeTeamList(TeamListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Team>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(team => new TeamView()
                {
                    Id = team.Id,
                    Description = team.Description,
                    Name = team.Name,
                    Website = team.Website,
                    Avatar = _avatarFactory.GetAvatar(team)
                });

            return new TeamList()
            {
                Organisation = listInput.OrganisationId != null ? _documentSession.Load<Organisation>(listInput.OrganisationId) : null,
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Teams = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private List<TeamView> GetGroupsHavingAddProjectPermission()
        {
            var loggedInUserId = _userContext.GetAuthenticatedUserId();

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => 
                    x.Id.In(_usersGroupsHavingPermissionQuery.GetUsersGroupsHavingPermission(loggedInUserId, "createproject")) &&
                    x.GroupType == "team"
                )
                .ToList()
                .Select(team => new TeamView()
                {
                    Id = team.Id,
                    Description = team.Team.Description,
                    Name = team.Team.Name,
                    Website = team.Team.Website,
                    Avatar = _avatarFactory.GetAvatar(team.Team)
                })
                .ToList();
        }

        #endregion
    }
}