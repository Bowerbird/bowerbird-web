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

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class ObservationNotesController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IObservationNotesViewModelBuilder _observationNotesViewModelBuilder;
        private readonly IObservationsViewModelBuilder _observationsViewModelBuilder;

        #endregion

        #region Constructors

        public ObservationNotesController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IObservationNotesViewModelBuilder observationNotesViewModelBuilder,
            IObservationsViewModelBuilder observationsViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(observationNotesViewModelBuilder, "observationNotesViewModelBuilder");
            Check.RequireNotNull(observationsViewModelBuilder, "observationsViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _observationNotesViewModelBuilder = observationNotesViewModelBuilder;
            _observationsViewModelBuilder = observationsViewModelBuilder;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            ViewBag.Observation = _observationsViewModelBuilder.BuildObservation(idInput);

            ViewBag.ObservationNote = _observationNotesViewModelBuilder.BuildObservationNote(idInput);

            return View(Form.Index);
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            ViewBag.ObservationNoteList = _observationNotesViewModelBuilder.BuildObservationNoteList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return new JsonNetResult(_observationNotesViewModelBuilder.BuildObservationNote(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            return new JsonNetResult(_observationNotesViewModelBuilder.BuildObservationNoteList(pagingInput));
        }
         
        /// <summary>
        /// idInput.Id = the Id of the Observation we wish to add the ObservationNote to
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(idInput.Id, PermissionNames.CreateObservationNote))
            {
                return HttpUnauthorized();
            }

            ViewBag.Observation = _observationsViewModelBuilder.BuildObservation(idInput);

            ViewBag.ObservationNote = new
            {
                ObservationId = idInput.Id,
                CreatedOn = DateTime.Now.ToString("d MMM yyyy"),
                CommonName = string.Empty,
                ScientificName = string.Empty,
                Taxonomy = string.Empty,
                Descriptions = new string[] {},
                References = new string[] {},
                Tags = new string[] {}
            };

            ViewBag.PrerenderedView = "observationnote";

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

            ViewBag.Observation = _observationsViewModelBuilder.BuildObservation(idInput);

            ViewBag.ObservationNote = _observationNotesViewModelBuilder.BuildObservationNote(idInput);

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasUserProjectPermission(PermissionNames.DeleteObservation))
            {
                return HttpUnauthorized();
            }

            ViewBag.Observation = _observationsViewModelBuilder.BuildObservation(idInput);

            ViewBag.ObservationNote = _observationNotesViewModelBuilder.BuildObservationNote(idInput);

            return View(Form.Delete);
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(ObservationNoteCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission<Observation>(createInput.ObservationId, PermissionNames.CreateObservationNote))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ObservationNoteCreateCommand()
                    {
                        ObservationId = createInput.ObservationId,
                        NotedOn = createInput.NotedOn,
                        UserId = _userContext.GetAuthenticatedUserId(),
                        CommonName = createInput.CommonName,
                        ScientificName = createInput.ScientificName,
                        Taxonomy = createInput.Taxonomy,
                        Descriptions = createInput.Descriptions,
                        References = createInput.References,
                        Tags = createInput.Tags
                    });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(ObservationNoteUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission<ObservationNote>(PermissionNames.UpdateObservation, updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ObservationNoteUpdateCommand
                {
                    Id = updateInput.Id,
                    NotedOn = updateInput.NotedOn,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    CommonName = updateInput.CommonName,
                    ScientificName = updateInput.ScientificName,
                    Taxonomy = updateInput.Taxonomy,
                    Descriptions = updateInput.Descriptions,
                    References = updateInput.References,
                    Tags = updateInput.Tags
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission<ObservationNote>(PermissionNames.DeleteObservationNote, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ObservationNoteDeleteCommand
                {
                    Id = idInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        #endregion
    }
}