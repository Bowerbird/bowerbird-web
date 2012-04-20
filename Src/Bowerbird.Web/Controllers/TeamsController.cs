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
using Bowerbird.Web.Config;
using Bowerbird.Web.Queries;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class TeamsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly ITeamsQuery _teamsQuery;

        #endregion

        #region Constructors

        public TeamsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            ITeamsQuery teamsQuery
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(teamsQuery, "teamsQuery");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _teamsQuery = teamsQuery;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(_teamsQuery.MakeTeamIndex(idInput));
                }

                return View(_teamsQuery.MakeTeamIndex(idInput));
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
                    return new JsonNetResult(_teamsQuery.TeamsHavingAddProjectPermission());
                }

                if (Request.IsAjaxRequest())
                {
                    return new JsonNetResult(_teamsQuery.MakeTeamList(listInput));
                }
            }

            ViewBag.Teams = _teamsQuery.MakeTeamList(new TeamListInput(){Page = 1, PageSize = 10}).Teams.PagedListItems;

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

            _commandProcessor.Process(
                new TeamCreateCommand()
                    {
                        Description = createInput.Description,
                        Name = createInput.Name,
                        UserId = _userContext.GetAuthenticatedUserId(),
                        OrganisationId = createInput.Organisation
                    }
                );

            return Json("success");
        }

        [Transaction]
        [Authorize]
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

            _commandProcessor.Process(
                new TeamUpdateCommand()
                {
                    Id = updateInput.Id,
                    Description = updateInput.Description,
                    Name = updateInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = updateInput.AvatarId
                }
            );

            return Json("Success");
        }

        [Transaction]
        [Authorize]
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

            _commandProcessor.Process(
                new TeamDeleteCommand()
                {
                    Id = deleteInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

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

            _commandProcessor.Process(
                new TeamProjectCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Name = projectCreateInput.Name,
                    Description = projectCreateInput.Description,
                    Administrators = teamProjectCreateInput.Administrators,
                    Members = teamProjectCreateInput.Members,
                    TeamId = teamProjectCreateInput.TeamId
                }
            );

            return Json("success");
        }

        #endregion
    }
}