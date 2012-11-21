/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using System.Web.Mvc;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Infrastructure;
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
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class ObservationsController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IDocumentSession _documentSession;
        private readonly IPermissionManager _permissionManager;

        #endregion

        #region Constructors

        public ObservationsController(
            IMessageBus messageBus,
            IUserContext userContext,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IDocumentSession documentSession,
            IPermissionManager permissionManager
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionManager, "permissionManager");

            _messageBus = messageBus;
            _userContext = userContext;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _documentSession = documentSession;
            _permissionManager = permissionManager;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Observation = _sightingViewModelBuilder.BuildSighting(observationId);

            return RestfulResult(
                viewModel,
                "observations",
                "index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string category = "", string projectId = "")
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            if (!string.IsNullOrWhiteSpace(projectId))
            {
                var project = _documentSession.Load<Project>(projectId);

                if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, project.Id))
                {
                    return HttpUnauthorized(); // TODO: Probably should return a soft user error suggesting user joins project
                }
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelBuilder.BuildNewObservation(category, projectId);
            viewModel.CategorySelectList = GetCategorySelectList(null, category);
            viewModel.ProjectsSelectList = GetProjectsSelectList(projectId);
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

            if (!_permissionManager.DoesExist<Observation>(observationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return HttpUnauthorized();
            }

            var observation = _documentSession.Load<Observation>(observationId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Observation = _sightingViewModelBuilder.BuildSighting(observationId);
            viewModel.CategorySelectList = GetCategorySelectList(observationId);
            viewModel.ProjectsSelectList = GetProjectsSelectList(observation.Groups.Where(x => x.Group.GroupType == "project").Select(x => x.Group.Id).ToArray());
            viewModel.Categories = GetCategories();

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

            if (!_permissionManager.DoesExist<Observation>(observationId))
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

            _messageBus.Send(
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
                        Projects = createInput.ProjectIds,
                        Media = createInput.Media.Select(x => new ObservationMediaUpdateCommand() 
                            { 
                                MediaResourceId = x.MediaResourceId, 
                                Key = x.Key,
                                Description = x.Description, 
                                Licence = x.Licence, 
                                IsPrimaryMedia = x.IsPrimaryMedia 
                            })
                    });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationUpdateInput updateInput)
        {
            string observationId = VerbosifyId<Observation>(updateInput.Id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
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

            _messageBus.Send(
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
                    Projects = updateInput.ProjectIds,
                    Media = updateInput.Media.Select(x => new ObservationMediaUpdateCommand()
                    {
                        MediaResourceId = x.MediaResourceId,
                        Description = x.Description,
                        Licence = x.Licence,
                        IsPrimaryMedia = x.IsPrimaryMedia
                    })
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!_permissionManager.DoesExist<Observation>(observationId))
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

            _messageBus.Send(
                new ObservationDeleteCommand
                {
                    Id = id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private IEnumerable GetCategorySelectList(string observationId = "", string category = "")
        {
            if (!string.IsNullOrWhiteSpace(observationId))
            {
                category = _documentSession.Load<Observation>(observationId).Category;
            }

            return _documentSession
                .Load<AppRoot>(Constants.AppRootId)
                .Categories
                .Select(x => new
                   {
                       Text = x.Name,
                       Value = x.Name,
                       Selected = x.Name == category
                   });
        }

        private IEnumerable GetProjectsSelectList(params string[] projectIds)
        {
            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .Single()
                .Projects
                .Select(x => new
                {
                    Text = x.Name,
                    Value = x.Id,
                    Selected = projectIds.Any(y => y == x.Id)
                });
        }

        private IEnumerable GetCategories()
        {
            return _documentSession
                .Load<AppRoot>(Constants.AppRootId)
                .Categories
                .ToList();
        }

        #endregion
    }
}