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
    using Core;
    using Core.Commands;
    using Core.DesignByContract;
    using ViewModels;
    using Config;

    #endregion

    public class ProjectObservationController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public ProjectObservationController(
            ICommandProcessor commandProcessor
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");

            _commandProcessor = commandProcessor;
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
            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ProjectObservationDeleteInput deleteInput)
        {
            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        private ProjectObservationCreateCommand MakeCreateCommand(ProjectObservationCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            return new ProjectObservationCreateCommand()
            {
                UserId = createInput.UserId,
                ObservationId = createInput.ObservationId,
                ProjectId = createInput.ProjectId
            };
        }

        private ProjectObservationDeleteCommand MakeDeleteCommand(ProjectObservationDeleteInput deleteInput)
        {
            Check.RequireNotNull(deleteInput, "deleteInput");

            return new ProjectObservationDeleteCommand()
            {
                UserId = deleteInput.UserId,
                ProjectId = deleteInput.ProjectId,
                ObservationId = deleteInput.ObservationId
            };
        }

        #endregion
    }
}