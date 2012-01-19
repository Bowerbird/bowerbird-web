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
using Bowerbird.Core;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class ProjectController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
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
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())
         
                return Json(MakeIndex(idInput));

            return View(MakeIndex(idInput));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(ProjectCreateInput createInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeCreateCommand(createInput));

                return Json("Success");
            }

            return Json("Failure");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(ProjectUpdateInput updateInput) 
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUpdateCommand(updateInput));

                return Json("Success");
            }

            return Json("Failure");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("Success");
            }
            return Json("Failure");
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
            return new ProjectCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private ProjectDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new ProjectDeleteCommand()
            {
                Id = deleteInput.Id,
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