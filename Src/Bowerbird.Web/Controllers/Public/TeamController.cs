/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Public  
{
    public class TeamController : Controller
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamController(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

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

        private TeamIndex MakeIndex(IdInput idInput)
        {
            var team = _documentSession.Load<Team>(idInput.Id);

            var projects =
                _documentSession
                .Load<Project>()
                .Where(x => x.Team.Id == idInput.Id)
                .ToList();

            return new TeamIndex()
            {
                Team = team,
                Projects = projects
            };
        }

        #endregion
    }
}