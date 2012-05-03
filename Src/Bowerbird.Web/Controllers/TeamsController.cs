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
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class TeamsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly ITeamsViewModelBuilder _viewModelBuilder;

        #endregion

        #region Constructors

        public TeamsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            ITeamsViewModelBuilder teamsViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(teamsViewModelBuilder, "viewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _viewModelBuilder = teamsViewModelBuilder;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            ViewBag.Team = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Index);
        }

        [HttpGet]
        public ActionResult Explore(TeamListInput listInput)
        {
            ViewBag.TeamList = _viewModelBuilder.BuildList(listInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return Json(_viewModelBuilder.BuildItem(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(TeamListInput listInput)
        {
            return Json(_viewModelBuilder.BuildList(listInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateTeam, idInput.Id))
            {
                return HttpUnauthorized();
            }

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateTeam))
            {
                return HttpUnauthorized();
            }

            ViewBag.Team = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteTeam))
            {
                return HttpUnauthorized();
            }

            ViewBag.Team = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Delete);
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
                return JsonFailed();
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

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(TeamUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.UpdateTeam, updateInput.Id))
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
                    Id = updateInput.Id,
                    Description = updateInput.Description,
                    Name = updateInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = updateInput.AvatarId
                }
            );

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteTeam, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new TeamDeleteCommand()
                {
                    Id = deleteInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
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
                return JsonFailed();
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

            return JsonSuccess();
        }

        #endregion
    }
}