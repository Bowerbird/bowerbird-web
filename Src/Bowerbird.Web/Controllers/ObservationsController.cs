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
using Bowerbird.Web.Builders;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using System;
using Bowerbird.Core.Config;
using Raven.Client;
using System.Collections;
using System.Dynamic;

namespace Bowerbird.Web.Controllers
{
    public class ObservationsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IDocumentSession _documentSession;
        private readonly IPermissionChecker _permissionChecker;

        #endregion

        #region Constructors

        public ObservationsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IDocumentSession documentSession,
            IPermissionChecker permissionChecker
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionChecker, "permissionChecker");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _documentSession = documentSession;
            _permissionChecker = permissionChecker;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionChecker.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            var viewModel = new
                {
                    Observation = _sightingViewModelBuilder.BuildSighting(observationId)
                };

            return RestfulResult(
                viewModel,
                "observations",
                "index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string id =  null)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (id != null)
            {
                var project = _documentSession.Load<Project>(id);

                if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, project.Id))
                {
                    return HttpUnauthorized();
                }
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = id != null ? _sightingViewModelBuilder.BuildNewObservationForProject(id) : _sightingViewModelBuilder.BuildNewObservation();
            
            viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel, 
                "observations", 
                "create", 
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionChecker.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return HttpUnauthorized();
            }

            var observation = _sightingViewModelBuilder.BuildSighting(observationId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = observation;
            viewModel.Categories = GetCategories(observationId);

            return RestfulResult(
                viewModel,
                "observations",
                "update", 
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionChecker.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelBuilder.BuildSighting(observationId);

            return RestfulResult(
                viewModel,
                "observations",
                "delete", 
                new Action<dynamic>(x => x.Model.Delete = true));
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationCreateInput createInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ObservationCreateCommand()
                    {
                        Title = createInput.Title,
                        Latitude = createInput.Latitude,
                        Longitude = createInput.Longitude,
                        Address = createInput.Address,
                        IsIdentificationRequired = createInput.IsIdentificationRequired,
                        AnonymiseLocation = createInput.AnonymiseLocation,
                        Category = createInput.Category,
                        ObservedOn = createInput.ObservedOn,
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Projects = createInput.Projects,
                        Media = createInput.Media.Select(x => new Tuple<string, string, string>(x.MediaResourceId, x.Description, x.Licence))
                    });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationUpdateInput updateInput)
        {
            string observationId = VerbosifyId<Observation>(updateInput.Id);

            if (!_permissionChecker.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ObservationUpdateCommand
                {
                    Id = observationId,
                    Title = updateInput.Title,
                    Latitude = updateInput.Latitude,
                    Longitude = updateInput.Longitude,
                    Address = updateInput.Address,
                    IsIdentificationRequired = updateInput.IsIdentificationRequired,
                    AnonymiseLocation = updateInput.AnonymiseLocation,
                    Category = updateInput.Category,
                    ObservedOn = updateInput.ObservedOn,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Projects = updateInput.Projects
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionChecker.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, observationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ObservationDeleteCommand
                {
                    Id = id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private IEnumerable GetCategories(string observationId = "")
        {
            var category = string.Empty;

            if (!string.IsNullOrWhiteSpace(observationId))
            {
                category = _documentSession.Load<Observation>(observationId).Category;
            }

            return _documentSession
                .Load<AppRoot>(Constants.AppRootId)
                .Categories
                .Select(x => new
                   {
                       Text = x,
                       Value = x,
                       Selected = x == category
                   });
        }

        #endregion
    }
}