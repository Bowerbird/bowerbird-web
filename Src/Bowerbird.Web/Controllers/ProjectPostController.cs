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

    #endregion

    public class ProjectPostController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public ProjectPostController(
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
        public ActionResult Create(ProjectPostCreateInput createInput)
        {
            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(ProjectPostUpdateInput updateInput)
        {
            _commandProcessor.Process(MakeUpdateCommand(updateInput));

            return Json("success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ProjectPostDeleteInput deleteInput)
        {
            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        private ProjectPostCreateCommand MakeCreateCommand(ProjectPostCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            return new ProjectPostCreateCommand()
            {
                UserId = createInput.UserId,
                ProjectId = createInput.ProjectId,
                MediaResources = createInput.MediaResources,
                Message = createInput.Message,
                Subject = createInput.Subject,
                Timestamp = createInput.Timestamp
            };
        }

        private ProjectPostDeleteCommand MakeDeleteCommand(ProjectPostDeleteInput deleteInput)
        {
            Check.RequireNotNull(deleteInput, "deleteInput");

            return new ProjectPostDeleteCommand()
            {
                UserId = deleteInput.UserId,
                Id = deleteInput.Id
            };
        }

        private ProjectPostUpdateCommand MakeUpdateCommand(ProjectPostUpdateInput updateInput)
        {
            Check.RequireNotNull(updateInput, "updateInput");

            return new ProjectPostUpdateCommand()
            {
                UserId = updateInput.UserId,
                Id = updateInput.Id,
                MediaResources = updateInput.MediaResources,
                Message = updateInput.Message,
                Subject = updateInput.Subject,
                Timestamp = updateInput.Timestamp
            };
        }

        #endregion
    }
}