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

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Public
{
    public class ObservationController : ControllerBase
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationController(
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            return View(MakeObservationIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ObservationListInput observationListInput)
        {
            if (observationListInput.ProjectId == null)
            {
                return Json(MakeObservationList(observationListInput), JsonRequestBehavior.AllowGet);
            }

            return Json(MakeObservationListByProjectId(observationListInput), JsonRequestBehavior.AllowGet);
        }

        private ObservationIndex MakeObservationIndex(IdInput idInput)
        {
            return new ObservationIndex()
                       {
                           Observation = _documentSession.Load<Observation>(idInput.Id)
                       };
        }

        private ObservationList MakeObservationList(ObservationListInput observationListInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray();
            // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ObservationList
            {
                Page = observationListInput.Page,
                PageSize = observationListInput.PageSize,
                Observations = results.ToPagedList(
                    observationListInput.Page,
                    observationListInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private ObservationList MakeObservationListByProjectId(ObservationListInput observationListInput)
        {
            RavenQueryStatistics stats;

            var projectObservations = _documentSession
                .Query<ProjectObservation>()
                .Where(x => x.Project.Id == observationListInput.ProjectId)
                .Customize(x => x.Include(observationListInput.ProjectId))
                .Statistics(out stats)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray();

            var results = _documentSession
                .Load<Observation>(projectObservations.Select(x => x.Observation.Id))
                .ToArray();

            return new ObservationList
            {
                Project = _documentSession.Load<Project>(observationListInput.ProjectId),
                Page = observationListInput.Page,
                PageSize = observationListInput.PageSize,
                Observations = results.ToPagedList(
                    observationListInput.Page,
                    observationListInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion      
    }
}