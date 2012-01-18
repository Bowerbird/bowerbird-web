/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Controllers.Public
{
    #region Namespaces

    using System.Web.Mvc;
    using System.Linq;

    using Raven.Client;
    using Raven.Client.Linq;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Web.ViewModels.Shared;
    using Bowerbird.Web.ViewModels.Public;

    #endregion

    public class ProjectController : Controller
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectController(
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

        #endregion
    }
}