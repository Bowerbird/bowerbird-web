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
using System;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class OrganisationsController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IOrganisationsViewModelBuilder _organisationsViewModelBuilder;
        private readonly IStreamItemsViewModelBuilder _streamItemsViewModelBuilder;
        private readonly ITeamsViewModelBuilder _teamsViewModelBuilder;
        private readonly IPostsViewModelBuilder _postsViewModelBuilder;
        private readonly IReferenceSpeciesViewModelBuilder _referenceSpeciesViewModelBuilder;

        #endregion

        #region Constructors

        public OrganisationsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IOrganisationsViewModelBuilder organisationsViewModelBuilder,
            IStreamItemsViewModelBuilder streamItemsViewModelBuilder,
            ITeamsViewModelBuilder teamsViewModelBuilder,
            IPostsViewModelBuilder postsViewModelBuilder,
            IReferenceSpeciesViewModelBuilder referenceSpeciesViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(organisationsViewModelBuilder, "organisationsViewModelBuilder");
            Check.RequireNotNull(streamItemsViewModelBuilder, "streamItemsViewModelBuilder");
            Check.RequireNotNull(teamsViewModelBuilder, "teamsViewModelBuilder");
            Check.RequireNotNull(postsViewModelBuilder, "postsViewModelBuilder");
            Check.RequireNotNull(referenceSpeciesViewModelBuilder, "referenceSpeciesViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _organisationsViewModelBuilder = organisationsViewModelBuilder;
            _streamItemsViewModelBuilder = streamItemsViewModelBuilder;
            _teamsViewModelBuilder = teamsViewModelBuilder;
            _postsViewModelBuilder = postsViewModelBuilder;
            _referenceSpeciesViewModelBuilder = referenceSpeciesViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Stream(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Team = _organisationsViewModelBuilder.BuildOrganisation(new IdInput() { Id = "organisations/" + pagingInput.Id }),
                StreamItems = _streamItemsViewModelBuilder.BuildGroupStreamItems(pagingInput)
            };

            ViewBag.PrerenderedView = "organisations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult StreamList(PagingInput pagingInput)
        {
            return new JsonNetResult(_streamItemsViewModelBuilder.BuildGroupStreamItems(pagingInput));
        }

        [HttpGet]
        public ActionResult ReferenceSpecies(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Team = _teamsViewModelBuilder.BuildTeam(new IdInput() { Id = "teams/" + pagingInput.Id }),
                ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildGroupReferenceSpeciesList(pagingInput)
            };

            ViewBag.PrerenderedView = "referencespecies"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Teams(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(new IdInput() { Id = "organisations/" + pagingInput.Id }),
                Teams = _teamsViewModelBuilder.BuildOrganisationTeamList(pagingInput)
            };

            ViewBag.PrerenderedView = "organisations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Posts(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(new IdInput() { Id = "organisations/" + pagingInput.Id }),
                Posts = _postsViewModelBuilder.BuildGroupPostList(pagingInput)
            };

            ViewBag.PrerenderedView = "organisations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Members(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(new IdInput() { Id = "organisations/" + pagingInput.Id }),
                Members = _organisationsViewModelBuilder.BuildOrganisationUserList(pagingInput)
            };

            ViewBag.PrerenderedView = "organisations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult About()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            ViewBag.OrganisationList = _organisationsViewModelBuilder.BuildOrganisationList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return new JsonNetResult(_organisationsViewModelBuilder.BuildOrganisation(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            return new JsonNetResult(_organisationsViewModelBuilder.BuildOrganisationList(pagingInput));
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

            ViewBag.Organisation = _organisationsViewModelBuilder.BuildOrganisation(idInput);

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

            ViewBag.Organisation = _organisationsViewModelBuilder.BuildOrganisation(idInput);

            return View(Form.Delete);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.JoinOrganisation, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new MemberCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = idInput.Id,
                    CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { RoleNames.OrganisationMember }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Leave(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            if (!_userContext.HasGroupPermission(PermissionNames.LeaveOrganisation, idInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new MemberDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = idInput.Id
                });

            return JsonSuccess();
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddTeam(GroupAssociationCreateInput createInput)
        {
            Check.RequireNotNull(createInput, "createInput");

            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.AddTeam, createInput.ParentGroupId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new GroupAssociationCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    ParentGroupId = createInput.ParentGroupId,
                    ChildGroupId = createInput.ChildGroupId
                });

            return JsonSuccess();
        }

        [HttpPost]
        [Authorize]
        public ActionResult RemoveTeam(GroupAssociationDeleteInput deleteInput)
        {
            Check.RequireNotNull(deleteInput, "deleteInput");

            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.RemoveTeam, deleteInput.ParentGroupId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _commandProcessor.Process(
                new GroupAssociationDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    ParentGroupId = deleteInput.ParentGroupId,
                    ChildGroupId = deleteInput.ChildGroupId
                });

            return JsonSuccess();
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