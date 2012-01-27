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
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class OrganisationController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationController(
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
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(MakeOrganisationIndex(idInput));
            }

            return View(MakeOrganisationIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(OrganisationListInput organisationListInput)
        {
            return Json(MakeOrganisationList(organisationListInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(OrganisationCreateInput createInput)
        {
            if (!_userContext.HasGlobalPermission(Permissions.CreateOrganisation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeOrganisationCreateCommand(createInput));

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(OrganisationUpdateInput updateInput)
        {
            if (!_userContext.HasPermissionToUpdate<Organisation>(updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeOrganisationUpdateCommand(updateInput));

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasPermissionToDelete<Organisation>(deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeOrganisationDeleteCommand(deleteInput));

            return Json("Success");
        }

        private OrganisationIndex MakeOrganisationIndex(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return new OrganisationIndex()
            {
                Organisation = _documentSession.Load<Organisation>(idInput.Id)
            };
        }

        private OrganisationList MakeOrganisationList(OrganisationListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Organisation>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new OrganisationList()
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Organisations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private OrganisationCreateCommand MakeOrganisationCreateCommand(OrganisationCreateInput createInput)
        {
            return new OrganisationCreateCommand()
            {
                Description = createInput.Description,
                Name = createInput.Name,
                Website = createInput.Website,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private OrganisationDeleteCommand MakeOrganisationDeleteCommand(IdInput deleteInput)
        {
            return new OrganisationDeleteCommand()
            {
                Id = deleteInput.Id,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private OrganisationUpdateCommand MakeOrganisationUpdateCommand(OrganisationUpdateInput updateInput)
        {
            return new OrganisationUpdateCommand()
            {
                Description = updateInput.Description,
                Name = updateInput.Name,
                Website = updateInput.Website,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        #endregion
    }
}