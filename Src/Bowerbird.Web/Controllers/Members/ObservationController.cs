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
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
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

    public class ObservationController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            return Json(MakeObservationIndex(idInput));
        }

        [HttpGet]
        public ActionResult List(ObservationListInput observationListInput)
        {
            return Json(MakeObservationList(observationListInput));
        }

        [Transaction]
        [HttpPost] 
        public ActionResult Create(ObservationCreateInput observationCreateInput)
        {
            _commandProcessor.Process(MakeObservationCreateCommand(observationCreateInput));

            return Json("success"); // TODO: Return something more meaningful?
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(ObservationUpdateInput observationUpdateInput)
        {
            throw new NotImplementedException();
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(ObservationDeleteInput observationDeleteInput)
        {
            throw new NotImplementedException();
        }

        private ObservationCreateCommand MakeObservationCreateCommand(ObservationCreateInput observationCreateInput)
        {
            Check.RequireNotNull(observationCreateInput, "observationCreateInput");

            return new ObservationCreateCommand()
            {
                Title = observationCreateInput.Title,
                Latitude = observationCreateInput.Latitude,
                Longitude = observationCreateInput.Longitude,
                Address = observationCreateInput.Address,
                IsIdentificationRequired = observationCreateInput.IsIdentificationRequired,
                MediaResources = observationCreateInput.MediaResources,
                ObservationCategory = observationCreateInput.ObservationCategory,
                ObservedOn = observationCreateInput.ObservedOn,
                UserId = observationCreateInput.UserId
            };
        }

        private ObservationIndex MakeObservationIndex(IdInput idInput)
        {
            return new ObservationIndex()
            {
                Observation = _documentSession.Load<Observation>(idInput.Id)
            };
        }

        public ObservationList MakeObservationList(ObservationListInput observationListInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Where(x => x.User.Id == observationListInput.UserId)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray();
            // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ObservationList
            {
                UserId = observationListInput.UserId,
                ProjectId = observationListInput.ProjectId,
                TeamId = observationListInput.TeamId,
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