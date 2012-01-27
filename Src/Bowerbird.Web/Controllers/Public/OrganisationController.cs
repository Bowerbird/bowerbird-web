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
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Public
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


        #endregion
    }
}