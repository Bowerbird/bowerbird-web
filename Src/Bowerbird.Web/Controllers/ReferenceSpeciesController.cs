/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class ReferenceSpeciesController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IReferenceSpeciesViewModelBuilder _viewModelBuilder;

        #endregion

        #region Constructors

        public ReferenceSpeciesController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IReferenceSpeciesViewModelBuilder referenceSpeciesViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(referenceSpeciesViewModelBuilder, "referenceSpeciesViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _viewModelBuilder = referenceSpeciesViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            ViewBag.ReferenceSpecies = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Index);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return Json(_viewModelBuilder.BuildItem(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            return Json(_viewModelBuilder.BuildList(pagingInput));
        }

        [HttpGet]
        public ActionResult CreateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateReferenceSpecies, idInput.Id))
            {
                return HttpUnauthorized();
            }

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.UpdateReferenceSpecies, idInput.Id))
            {
                return HttpUnauthorized();
            }

            ViewBag.ReferenceSpecies = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteReferenceSpecies, idInput.Id))
            {
                return HttpUnauthorized();
            }

            ViewBag.ReferenceSpecies = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Delete);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(ReferenceSpeciesCreateInput createInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.CreateReferenceSpecies, createInput.GroupId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ReferenceSpeciesCreateCommand()
                {
                    SpeciesId = createInput.SpeciesId,
                    GroupId = createInput.GroupId,
                    CreatedOn = createInput.CreatedOn,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    SmartTags = createInput.SmartTags
                });

            return JsonSuccess();
        }

        [HttpPut]
        [Authorize]
        [Transaction]
        public ActionResult Update(ReferenceSpeciesUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission<ReferenceSpecies>(PermissionNames.DeleteReferenceSpecies, updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new ReferenceSpeciesUpdateCommand()
                {
                    Id = updateInput.Id,
                    SpeciesId = updateInput.SpeciesId,
                    GroupId = updateInput.GroupId,
                    ModifiedOn = updateInput.UpdatedOn,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    SmartTags = updateInput.SmartTags
                });

            return JsonSuccess();
        }

        [HttpDelete]
        [Authorize]
        public ActionResult Delete(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission<ReferenceSpecies>(PermissionNames.DeleteReferenceSpecies, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new DeleteCommand
                {
                    Id = idInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        #endregion
    }
}