/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Controllers.Members
{
    #region Namespaces

    using System.Web.Mvc;
    using System.Linq;

    using Raven.Client;
    using Raven.Client.Linq;
    
    using Core;
    using Core.Commands;
    using Core.DesignByContract;
    using ViewModels;
    using Config;
    using Core.Tasks;
    using Core.DomainModels;
    using ViewModels.Shared;
    using ViewModels.Members;

    #endregion

    public class ProjectController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserTasks _userTasks;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectController(
            ICommandProcessor commandProcessor,
            IUserTasks userTasks,
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userTasks, "userTasks");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userTasks = userTasks;
            _userContext = userContext;
            _documentSession = documentSession;
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

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            return View(MakeIndex(idInput));
        }

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

        private ProjectIndex MakeIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var project = _documentSession.Load<Project>(idInput.Id);

            var projectObservations =
                _documentSession
                .Query<ProjectObservation>()
                .Customize(x => x.Include(idInput.Id))
                .Where(x => x.Project.Id == idInput.Id)
                .ToList();

            var observations =
                _documentSession
                .Load<Observation>(projectObservations.Select(x => x.Id))
                .ToList();

            return new ProjectIndex()
            {
                Project = project,
                Observations = observations
            };
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