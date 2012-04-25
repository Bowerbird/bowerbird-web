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
    public class ProjectsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IProjectsQuery _projectsQuery;

        #endregion

        #region Constructors

        public ProjectsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IProjectsQuery projectsQuery
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectsQuery, "projectsQuery");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _projectsQuery = projectsQuery;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(_projectsQuery.MakeProjectIndex(idInput));
            }

            return View(_projectsQuery.MakeProjectIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ProjectListInput listInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (listInput.UserId != null)
                {
                    return Json(_projectsQuery.MakeProjectListByMembership(listInput), JsonRequestBehavior.AllowGet);
                }

                if (Request.IsAjaxRequest())
                {
                    return Json(_projectsQuery.MakeProjectList(listInput), JsonRequestBehavior.AllowGet);
                }
            }

            ViewBag.ProjectList = _projectsQuery.MakeProjectList(new ProjectListInput() { Page = 1, PageSize = 10 });

            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateProject, createInput.Team ?? Constants.AppRootId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(
                new ProjectCreateCommand()
                {
                    Description = createInput.Description,
                    Name = createInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = createInput.Avatar,
                    TeamId = createInput.Team
                });
            
            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(ProjectUpdateInput updateInput) 
        {
            if (!_userContext.HasGroupPermission<Project>(PermissionNames.UpdateProject, updateInput.ProjectId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }
            
            _commandProcessor.Process(
                new ProjectUpdateCommand()
                {
                    Description = updateInput.Description,
                    Name = updateInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = updateInput.AvatarId
                });

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasGroupPermission<Project>(PermissionNames.DeleteProject, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(
                new ProjectDeleteCommand()
                {
                    Id = deleteInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return Json("Success");
        }

        #endregion
    }
}