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
            if (Request.IsAjaxRequest())
            {
                return Json(MakeObservationIndex(idInput));
            }

            return View(MakeObservationIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ObservationListInput observationListInput)
        {
            if (observationListInput.GroupId != null)
            {
                return Json(MakeObservationListByProjectId(observationListInput), JsonRequestBehavior.AllowGet);
            }

            if(observationListInput.CreatedByUserId != null )
            {
                return Json(MakeObservationListByCretedByUserId(observationListInput), JsonRequestBehavior.AllowGet);
            }

            return Json(MakeObservationList(observationListInput));
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

        private ObservationList MakeObservationListByProjectId(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Observation>()
                .Where(x => x.GroupContributions.Any(y => y.GroupId == listInput.GroupId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray();
            // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ObservationList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Project = listInput.GroupId != null ? _documentSession.Load<Project>(listInput.GroupId) : null,
                Observations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private ObservationList MakeObservationListByCretedByUserId(ObservationListInput observationListInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Observation>()
                .Customize(x => x.Include(observationListInput.CreatedByUserId))
                .Where(x => x.User.Id == observationListInput.CreatedByUserId)
                .Statistics(out stats)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray();
            // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ObservationList
            {
                Page = observationListInput.Page,
                PageSize = observationListInput.PageSize,
                CreatedByUser = _documentSession.Load<User>(observationListInput.CreatedByUserId),
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