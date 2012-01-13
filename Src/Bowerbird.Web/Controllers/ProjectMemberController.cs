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

    using System;
    using System.Web.Mvc;
    using Bowerbird.Core;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.Config;

    #endregion

    public class ProjectMemberController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public ProjectMemberController(
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
        public ActionResult Create(ProjectMemberCreateInput createInput)
        {
            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ProjectMemberDeleteInput deleteInput)
        {
            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        private ProjectMemberCreateCommand MakeCreateCommand(ProjectMemberCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            return new ProjectMemberCreateCommand()
                       {
                           UserId = createInput.UserId
                       };
        }

        private ProjectMemberDeleteCommand MakeDeleteCommand(ProjectMemberDeleteInput deleteInput)
        {
            Check.RequireNotNull(deleteInput, "deleteInput");

            return new ProjectMemberDeleteCommand()
                       {
                           UserId = deleteInput.UserId
                       };
        }

        #endregion
    }
}