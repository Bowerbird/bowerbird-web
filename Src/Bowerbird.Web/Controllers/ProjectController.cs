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
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class ProjectController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ProjectController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
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
                return Json(MakeProjectIndex(idInput));
            }

            return View(MakeProjectIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ProjectListInput listInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (listInput.UserId != null)
                {
                    return Json(MakeProjectListByMembership(listInput), JsonRequestBehavior.AllowGet);
                }

                if (Request.IsAjaxRequest())
                {
                    return Json(MakeProjectList(listInput), JsonRequestBehavior.AllowGet);
                }
            }

            ViewBag.ProjectList = MakeProjectList(new ProjectListInput() { Page = 1, PageSize = 10 });

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

            _commandProcessor.Process(MakeProjectCreateCommand(createInput));
            
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
            
            _commandProcessor.Process(MakeProjectUpdateCommand(updateInput));

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

            _commandProcessor.Process(MakeProjectDeleteCommand(deleteInput));

            return Json("Success");
        }

        private ProjectCreateCommand MakeProjectCreateCommand(ProjectCreateInput createInput)
        {
            return new ProjectCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId(),
                AvatarId = createInput.Avatar,
                TeamId = createInput.Team
            };
        }

        private ProjectDeleteCommand MakeProjectDeleteCommand(IdInput deleteInput)
        {
            return new ProjectDeleteCommand()
            {
                Id = deleteInput.Id,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectUpdateCommand MakeProjectUpdateCommand(ProjectUpdateInput updateInput)
        {
            return new ProjectUpdateCommand()
            {
                Description = updateInput.Description,
                Name = updateInput.Name,
                UserId = _userContext.GetAuthenticatedUserId(),
                AvatarId = updateInput.AvatarId
            };
        }

        protected ProjectIndex MakeProjectIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession.Load<Project>(idInput.Id);

            return new ProjectIndex()
            {
                Project = project,
                Avatar = _avatarFactory.GetAvatar(project)
            };
        }

        protected ProjectList MakeProjectList(ProjectListInput listInput)
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

            return new ProjectList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Projects = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        protected ProjectList MakeProjectListByMembership(ProjectListInput listInput)
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