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
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Members
{
    public class OrganisationPostController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationPostController(
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
        public ActionResult Create(OrganisationPostCreateInput createInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeCreateCommand(createInput));

                return Json("success");
            }

            return Json("Failure");
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(OrganisationPostUpdateInput updateInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUpdateCommand(updateInput));

                return Json("success");
            }

            return Json("Failure");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeDeleteCommand(deleteInput));

                return Json("success");
            }

            return Json("Failure");
        }

        private OrganisationPostCreateCommand MakeCreateCommand(OrganisationPostCreateInput createInput)
        {
            return new OrganisationPostCreateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                OrganisationId = createInput.OrganisationId,
                MediaResources = createInput.MediaResources,
                Message = createInput.Message,
                Subject = createInput.Subject,
                PostedOn = createInput.Timestamp
            };
        }

        private OrganisationPostDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new OrganisationPostDeleteCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Id = deleteInput.Id
            };
        }

        private OrganisationPostUpdateCommand MakeUpdateCommand(OrganisationPostUpdateInput updateInput)
        {
            return new OrganisationPostUpdateCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
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