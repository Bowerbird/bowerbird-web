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

using System.Web.Mvc;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Controllers.Members
{
    public class ObservationController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IDocumentSession _documentSession;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ObservationController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IUserContext userContext)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        [Authorize]
        public ActionResult Index(IdInput idInput)
        {
            return Json(MakeObservationIndex(idInput), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult List(ObservationListInput observationListInput)
        {
            return Json(MakeObservationList(observationListInput), JsonRequestBehavior.AllowGet);
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationCreateInput observationCreateInput)
        {
            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(MakeObservationCreateCommand(observationCreateInput));

            return Json("success"); // TODO: Return something more meaningful?
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationUpdateInput observationUpdateInput)
        {
            if (!_userContext.HasPermissionToUpdate<Observation>(observationUpdateInput.ObservationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(MakeObservationUpdateCommand(observationUpdateInput));

            return Json("success"); // TODO: Return something more meaningful?                
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(IdInput idInput)
        {
            if (!_userContext.HasPermissionToDelete<Observation>(idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(MakeObservationDeleteCommand(idInput));

            return Json("success"); // TODO: Return something more meaningful?
        }

        private ObservationCreateCommand MakeObservationCreateCommand(ObservationCreateInput observationCreateInput)
        {
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
                           UserId = _userContext.GetAuthenticatedUserId()
                       };
        }

        private ObservationUpdateCommand MakeObservationUpdateCommand(ObservationUpdateInput observationUpdateInput)
        {
            return new ObservationUpdateCommand
                       {
                           Id = observationUpdateInput.ObservationId,
                           Title = observationUpdateInput.Title,
                           Latitude = observationUpdateInput.Latitude,
                           Longitude = observationUpdateInput.Longitude,
                           Address = observationUpdateInput.Address,
                           IsIdentificationRequired = observationUpdateInput.IsIdentificationRequired,
                           MediaResources = observationUpdateInput.MediaResources,
                           ObservationCategory = observationUpdateInput.ObservationCategory,
                           ObservedOn = observationUpdateInput.ObservedOn,
                           UserId = _userContext.GetAuthenticatedUserId()
                       };
        }

        private ObservationDeleteCommand MakeObservationDeleteCommand(IdInput idInput)
        {
            return new ObservationDeleteCommand
                       {
                           ObservationId =  idInput.Id,
                           UserId = _userContext.GetAuthenticatedUserId()
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
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

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