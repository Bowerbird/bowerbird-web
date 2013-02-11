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
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Queries;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using System;
using Bowerbird.Core.Config;
using Bowerbird.Web.Infrastructure;
using Raven.Client;
using System.Collections;
using System.Dynamic;

namespace Bowerbird.Web.Controllers
{
    public class RecordsController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly ISightingViewModelQuery _sightingViewModelQuery;
        private readonly IDocumentSession _documentSession;
        private readonly IPermissionManager _permissionManager;

        #endregion

        #region Constructors

        public RecordsController(
            IMessageBus messageBus,
            IUserContext userContext,
            ISightingViewModelQuery sightingViewModelQuery,
            IDocumentSession documentSession,
            IPermissionManager permissionManager
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(sightingViewModelQuery, "sightingViewModelQuery");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(permissionManager, "permissionManager");

            _messageBus = messageBus;
            _userContext = userContext;
            _sightingViewModelQuery = sightingViewModelQuery;
            _documentSession = documentSession;
            _permissionManager = permissionManager;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string id)
        {
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionManager.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            var viewModel = new
                {
                    Record = _sightingViewModelQuery.BuildSighting(recordId)
                };

            return RestfulResult(
                viewModel,
                "records",
                "index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(string id = null)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return new HttpUnauthorizedResult();
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                var project = _documentSession.Load<Project>(id);

                if (!_userContext.HasGroupPermission(PermissionNames.CreateObservation, project.Id))
                {
                    return new HttpUnauthorizedResult();
                }
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Record = _sightingViewModelQuery.BuildCreateRecord(id);
            viewModel.Create = true;
            //viewModel.Categories = GetCategories();

            return RestfulResult(
                viewModel,
                "records", 
                "create");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionManager.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return new HttpUnauthorizedResult();
            }

            var record = _sightingViewModelQuery.BuildSighting(recordId);

            dynamic viewModel = new ExpandoObject();

            viewModel.Record = record;
            viewModel.Update = true;
            //viewModel.Categories = GetCategories(recordId);

            return RestfulResult(
                viewModel,
                "records",
                "update");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionManager.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return new HttpUnauthorizedResult();
            }

            dynamic viewModel = new ExpandoObject();

            viewModel.Record = _sightingViewModelQuery.BuildSighting(recordId);

            return RestfulResult(
                viewModel,
                "records",
                "delete");
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(RecordUpdateInput createInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new RecordCreateCommand()
                    {
                        Latitude = createInput.Latitude,
                        Longitude = createInput.Longitude,
                        AnonymiseLocation = createInput.AnonymiseLocation,
                        Category = createInput.Category,
                        ObservedOn = createInput.ObservedOn,
                        UserId = _userContext.GetAuthenticatedUserId(),
                        //Projects = createInput.ProjectIds
                    });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(RecordUpdateInput updateInput)
        {
            string recordId = VerbosifyId<Record>(updateInput.Id);

            if (!_permissionManager.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Record>(PermissionNames.UpdateObservation, recordId))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new RecordUpdateCommand
                {
                    Id = recordId,
                    Latitude = updateInput.Latitude,
                    Longitude = updateInput.Longitude,
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
            string recordId = VerbosifyId<Record>(id);

            if (!_permissionManager.DoesExist<Record>(recordId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission<Record>(PermissionNames.UpdateObservation, recordId))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new RecordDeleteCommand
                {
                    Id = id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        //private IEnumerable GetCategories(string recordId = "")
        //{
        //    var category = string.Empty;

        //    if (!string.IsNullOrWhiteSpace(recordId))
        //    {
        //        category = _documentSession.Load<Record>(recordId).Category;
        //    }

        //    viewModel.CategorySelectList = Categories.GetSelectList(queryInput.Category ?? string.Empty);
        //    viewModel.Categories = Categories.GetAll();

        //    //return _documentSession
        //    //    .Load<AppRoot>(Constants.AppRootId)
        //    //    .Categories
        //    //    .Select(x => new
        //    //       {
        //    //           Text = x.Name,
        //    //           Value = x.Name,
        //    //           Selected = x.Name == category
        //    //       });
        //}

        #endregion
    }
}