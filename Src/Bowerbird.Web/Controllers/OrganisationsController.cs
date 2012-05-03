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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class OrganisationsController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IOrganisationsViewModelBuilder _viewModelBuilder;

        #endregion

        #region Constructors

        public OrganisationsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IOrganisationsViewModelBuilder organisationsViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(organisationsViewModelBuilder, "organisationsViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _viewModelBuilder = organisationsViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            ViewBag.Organisation = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Index);
        }

        [HttpGet]
        public ActionResult Explore(OrganisationListInput listInput)
        {
            ViewBag.OrganisationList = _viewModelBuilder.BuildList(listInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return Json(_viewModelBuilder.BuildItem(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(OrganisationListInput listInput)
        {
            return Json(_viewModelBuilder.BuildList(listInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            if (!_userContext.HasAppRootPermission(PermissionNames.CreateOrganisation))
            {
                return HttpUnauthorized();
            }

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            if (!_userContext.HasGroupPermission(PermissionNames.UpdateOrganisation, idInput.Id))
            {
                return HttpUnauthorized();
            }

            ViewBag.Organisation = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            if (!_userContext.HasAppRootPermission(PermissionNames.DeleteOrganisation))
            {
                return HttpUnauthorized();
            }

            ViewBag.Organisation = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Delete);
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(OrganisationCreateInput createInput)
        {
            if (!_userContext.HasAppRootPermission(PermissionNames.CreateOrganisation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new OrganisationCreateCommand()
                {
                    AvatarId = createInput.AvatarId,
                    Description = createInput.Description,
                    Name = createInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Website = createInput.Website
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(OrganisationUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.UpdateOrganisation, updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new OrganisationUpdateCommand
                {
                    AvatarId = updateInput.AvatarId,
                    Description = updateInput.Description,
                    Id = updateInput.Id,
                    Name = updateInput.Name,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Website = updateInput.Website
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(IdInput idInput)
        {
            if (!_userContext.HasAppRootPermission(PermissionNames.DeleteOrganisation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new OrganisationDeleteCommand
                {
                    Id = idInput.Id,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        #endregion
    }
}