/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Controllers
{
    #region Namespaces

    using System.Web.Mvc;

    using Bowerbird.Core;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.Config;
    using Bowerbird.Core.Tasks;

    #endregion

    public class ProjectController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserTasks _userTasks;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ProjectController(
            ICommandProcessor commandProcessor,
            IUserTasks userTasks,
            IUserContext userContext)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userTasks, "userTasks");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _userTasks = userTasks;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(int? id, int? page, int? pageSize)
        {
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult Index(ProjectIndexInput indexInput)
        //{
        //    //return View(_viewModelRepository.Load<ProjectIndexInput, ProjectIndex>(indexInput));
        //}

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(ProjectUpdateInput updateInput)
        {
            _commandProcessor.Process(MakeUpdateCommand(updateInput));

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(ProjectDeleteInput deleteInput)
        {
            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        private ProjectCreateCommand MakeCreateCommand(ProjectCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            return new ProjectCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectDeleteCommand MakeDeleteCommand(ProjectDeleteInput deleteInput)
        {
            Check.RequireNotNull(deleteInput, "deleteInput");

            return new ProjectDeleteCommand()
            {
                Id = deleteInput.ProjectId,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectUpdateCommand MakeUpdateCommand(ProjectUpdateInput updateInput)
        {
            return new ProjectUpdateCommand()
            {
                Description = updateInput.Description,
                Name = updateInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        #endregion
    }
}