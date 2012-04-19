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
using Bowerbird.Web.Queries;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using System;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class ObservationsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IObservationsQuery _observationsQuery;

        #endregion

        #region Constructors

        public ObservationsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IObservationsQuery observationsQuery
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(observationsQuery, "observationsQuery");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _observationsQuery = observationsQuery;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(_observationsQuery.MakeObservationIndex(idInput));
            }

            return View("Index", _observationsQuery.MakeObservationIndex(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(ObservationListInput observationListInput)
        {
            if (observationListInput.GroupId != null)
            {
                return Json(_observationsQuery.MakeObservationListByProjectId(observationListInput), JsonRequestBehavior.AllowGet);
            }

            if (observationListInput.CreatedByUserId != null)
            {
                return Json(_observationsQuery.MakeObservationListByCreatedByUserId(observationListInput), JsonRequestBehavior.AllowGet);
            }

            return Json(_observationsQuery.MakeObservationList(observationListInput));
        }
         
        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            return View("Create");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return HttpUnauthorized();
            }

            return View("Update");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return HttpUnauthorized();
            }

            return View("Delete");
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationCreateInput observationCreateInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(
                new ObservationCreateCommand()
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
                    });

            return Json("success"); // TODO: Return something more meaningful?
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationUpdateInput observationUpdateInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationUpdateInput.ObservationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(
                new ObservationUpdateCommand
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
                });

            return Json("success"); // TODO: Return something more meaningful?                
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.DeleteObservation, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("failure");
            }

            _commandProcessor.Process(
                new ObservationDeleteCommand
                {
                    ObservationId = idInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return Json("success"); // TODO: Return something more meaningful?
        }

        #endregion
    }
}