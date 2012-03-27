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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Raven.Client.Linq;
using System;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers.Members
{
    public class ObservationController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

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

            if (observationListInput.CreatedByUserId != null)
            {
                return Json(MakeObservationListByCretedByUserId(observationListInput), JsonRequestBehavior.AllowGet);
            }

            return Json(MakeObservationList(observationListInput));
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationCreateInput observationCreateInput)
        {
            if (!_userContext.HasGlobalPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

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

        private ObservationList MakeObservationListByCretedByUserId(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Observation>()
                .Customize(x => x.Include(listInput.CreatedByUserId))
                .Where(x => x.User.Id == listInput.CreatedByUserId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray();
            // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ObservationList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                CreatedByUser = _documentSession.Load<User>(listInput.CreatedByUserId),
                Observations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
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
                           Category = observationCreateInput.Category,
                           ObservedOn = observationCreateInput.ObservedOn,
                           UserId = _userContext.GetAuthenticatedUserId(),
                           Projects = observationCreateInput.Projects,
                           AddMedia = observationCreateInput.AddMedia.Select(x => new Tuple<string, string, string>(x.MediaResourceId, x.Description, x.Licence))
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
                           Category = observationUpdateInput.Category,
                           ObservedOn = observationUpdateInput.ObservedOn,
                           UserId = _userContext.GetAuthenticatedUserId(),
                           Projects = observationUpdateInput.Projects
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

        #endregion
    }
}