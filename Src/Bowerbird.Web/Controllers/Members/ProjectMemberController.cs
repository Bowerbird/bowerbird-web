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
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Members
{
    public class ProjectMemberController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectMemberController(
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
        public ActionResult Create(ProjectMemberCreateInput createInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeCreateCommand(createInput));

                return Json("success");
            }

            return Json("Failure");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ProjectMemberDeleteInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("success");
            }

            return Json("Failure");
        }

        private ProjectMemberCreateCommand MakeCreateCommand(ProjectMemberCreateInput createInput)
        {
            return new ProjectMemberCreateCommand()
            {
                UserId = createInput.UserId,
                CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                ProjectId = createInput.ProjectId,
                Roles = createInput.Roles.ToList()
            };
        }

        private ProjectMemberDeleteCommand MakeDeleteCommand(ProjectMemberDeleteInput deleteInput)
        {
            return new ProjectMemberDeleteCommand()
            {
                UserId = deleteInput.UserId,
                DeletedByUserId = _userContext.GetAuthenticatedUserId(),
                ProjectId = deleteInput.ProjectId
            };
        }

        #endregion
    }
}