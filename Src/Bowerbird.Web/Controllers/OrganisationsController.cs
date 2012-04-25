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
using Bowerbird.Web.Config;
using Bowerbird.Web.Queries;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class OrganisationsController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IOrganisationsQuery _organisationsQuery;

        #endregion

        #region Constructors

        public OrganisationsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IOrganisationsQuery organisationsQuery
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(organisationsQuery, "organisationsQuery");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _organisationsQuery = organisationsQuery;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(_organisationsQuery.MakeOrganisationIndex(idInput));
                }

                return View(_organisationsQuery.MakeOrganisationIndex(idInput));
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List(OrganisationListInput listInput)
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (listInput.HasAddTeamPermission)
                {
                    return new JsonNetResult(_organisationsQuery.GetGroupsHavingAddTeamPermission());
                }

                if(Request.IsAjaxRequest())
                {
                    return new JsonNetResult(_organisationsQuery.MakeOrganisationList(listInput));
                }
            }

            return View();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(OrganisationCreateInput createInput)
        {
            if (!_userContext.HasAppRootPermission(PermissionNames.CreateOrganisation))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(
                new OrganisationCreateCommand()
                {
                    Description = createInput.Description,
                    Name = createInput.Name,
                    Website = createInput.Website,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = createInput.AvatarId
                });

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpPut]
        public ActionResult Update(OrganisationUpdateInput updateInput)
        {
            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.UpdateOrganisation, updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(
                new OrganisationUpdateCommand()
                {
                    Id = updateInput.Id,
                    Description = updateInput.Description,
                    Name = updateInput.Name,
                    Website = updateInput.Website,
                    UserId = _userContext.GetAuthenticatedUserId(),
                    AvatarId = updateInput.AvatarId
                });

            return Json("Success");
        }

        [Transaction]
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.DeleteOrganisation, deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(
               new OrganisationDeleteCommand()
               {
                   Id = deleteInput.Id,
                   UserId = _userContext.GetAuthenticatedUserId()
               });

            return Json("Success");
        }

        #endregion
    }
}