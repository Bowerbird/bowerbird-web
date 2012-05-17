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

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class ObservationsController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IObservationsViewModelBuilder _viewModelBuilder;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IObservationsViewModelBuilder observationsViewModelBuilder,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(observationsViewModelBuilder, "observationsViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _viewModelBuilder = observationsViewModelBuilder;
            _documentSession = documentSession;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            ViewBag.Observation = _viewModelBuilder.BuildObservation(idInput);

            return View(Form.Index);
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            ViewBag.ObservationList = _viewModelBuilder.BuildObservationList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return new JsonNetResult(_viewModelBuilder.BuildObservation(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            return new JsonNetResult(_viewModelBuilder.BuildObservationList(pagingInput));
        }
         
        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.CreateObservation))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new
            {
                Create = true,
                Observation = _viewModelBuilder.BuildObservation(),
                Categories = GetCategories()
            };

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new { Model = ViewBag.Model });
            }

            ViewBag.PrerenderedView = "observations";

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.UpdateObservation))
            {
                return HttpUnauthorized();
            }

            ViewBag.Model = new 
            {
                Update = true,
                Observation = _viewModelBuilder.BuildObservation(idInput),
                Categories = GetCategories(idInput.Id)
            };

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new { Model = ViewBag.Model });
            }

            ViewBag.PrerenderedView = "observations";

            return View(Form.Update);
        }

        // HACK: Add to DB!
        private IEnumerable GetCategories(string id = "") {
            var categories = new [] { "Mammals", "Invertebrates", "Turkeys", "Apples" };
            var observation = _documentSession.Load<Observation>("observations/" + id);
            Func<string, bool> isSelected = null;

            if (observation != null)
            {
                isSelected = x => { return x == observation.Category; };
            }
            else
            {
                isSelected = x => { return false; };
            }

            return from category in categories
                   select new 
                   {
                        Text = category,
                        Value = category,
                        Selected = isSelected(category)
                   };
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return HttpUnauthorized();
            }

            ViewBag.Observation = _viewModelBuilder.BuildObservation(idInput);

            return View(Form.Delete);
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
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, updateInput.ObservationId))
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
                    Id = updateInput.ObservationId,
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
        public ActionResult Delete(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(PermissionNames.UpdateObservation, idInput.Id))
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
                    Id = idInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        #endregion
    }
}