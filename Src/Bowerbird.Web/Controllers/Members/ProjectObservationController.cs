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
using Bowerbird.Core;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Members
{
    public class ProjectObservationController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectObservationController(
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
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        public ActionResult Create(ProjectObservationCreateInput createInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeCreateCommand(createInput));

                return Json("success");
            }
            return Json("failure");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ProjectObservationDeleteInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("success");
            }
            return Json("failure");
        }

        private ProjectObservationCreateCommand MakeCreateCommand(ProjectObservationCreateInput createInput)
        {
            return new ProjectObservationCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                ObservationId = createInput.ObservationId,
                ProjectId = createInput.ProjectId
            };
        }

        private ProjectObservationDeleteCommand MakeDeleteCommand(ProjectObservationDeleteInput deleteInput)
        {
            return new ProjectObservationDeleteCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                ProjectId = deleteInput.ProjectId,
                ObservationId = deleteInput.ObservationId
            };
        }

        #endregion
    }
}