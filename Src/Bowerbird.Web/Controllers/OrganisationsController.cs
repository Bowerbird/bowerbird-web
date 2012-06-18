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
using Bowerbird.Core.Extensions;
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
            Check.RequireNotNull(pagingInput, "pagingInput");

            var organisationId = "organisations/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(organisationId),
                StreamItems = _streamItemsViewModelBuilder.BuildGroupStreamItems(null, null, pagingInput)
            };

            ViewBag.PrerenderedView = "organisations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult StreamList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            return new JsonNetResult(_streamItemsViewModelBuilder.BuildGroupStreamItems(null, null, pagingInput));
        }

        [HttpGet]
        public ActionResult ReferenceSpecies(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var organisationId = "organisations/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(organisationId),
                ReferenceSpecies = _referenceSpeciesViewModelBuilder.BuildGroupReferenceSpeciesList(pagingInput)
            };

            ViewBag.PrerenderedView = "referencespecies"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Teams(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var organisationId = "organisations/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(organisationId),
                Teams = _teamsViewModelBuilder.BuildOrganisationTeamList(pagingInput)
            };

            ViewBag.PrerenderedView = "organisations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Posts(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var organisationId = "organisations/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(organisationId),
                Posts = _postsViewModelBuilder.BuildGroupPostList(pagingInput)
            };

            ViewBag.PrerenderedView = "organisations"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Post(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisationId = "organisations/".AppendWith(idInput.Id);

            ViewBag.Model = new
            {
                Post = _postsViewModelBuilder.BuildPost(organisationId)
            };

            ViewBag.PrerenderedView = "post"; // HACK: Need to rethink this

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Members(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            var organisationId = "organisations/".AppendWith(pagingInput.Id);

            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation(organisationId),
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
            Check.RequireNotNull(pagingInput, "pagingInput");

            ViewBag.OrganisationList = _organisationsViewModelBuilder.BuildOrganisationList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisationId = "organisations/".AppendWith(idInput.Id);

            return new JsonNetResult(_organisationsViewModelBuilder.BuildOrganisation(organisationId));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

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

            ViewBag.Model = new
            {
                Organisation = _organisationsViewModelBuilder.BuildOrganisation()
            };

            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = ViewBag.Model
                });
            }

            ViewBag.PrerenderedView = "organisations";

            return View(Form.Create);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisationId = "organisations/".AppendWith(idInput.Id);

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateOrganisation, organisationId))
            {
                return HttpUnauthorized();
            }

            ViewBag.Organisation = _organisationsViewModelBuilder.BuildOrganisation(organisationId);

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisationId = "organisations/".AppendWith(idInput.Id);

            if (!_userContext.HasAppRootPermission(PermissionNames.DeleteOrganisation))
            {
                return HttpUnauthorized();
            }

            ViewBag.Organisation = _organisationsViewModelBuilder.BuildOrganisation(organisationId);

            return View(Form.Delete);
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var organisationId = "organisations/".AppendWith(idInput.Id);

            if (!_userContext.HasGroupPermission(PermissionNames.JoinOrganisation, organisationId))
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
                    GroupId = organisationId,
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

            var organisationId = "organisations/".AppendWith(idInput.Id);

            if (!_userContext.HasGroupPermission(PermissionNames.LeaveOrganisation, organisationId))
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

            var organisationId = "organisations/".AppendWith(createInput.ParentGroupId);

            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.AddTeam, organisationId))
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

            var organisationId = "organisations/".AppendWith(deleteInput.ParentGroupId);

            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.RemoveTeam, organisationId))
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
            Check.RequireNotNull(createInput, "createInput");

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
            Check.RequireNotNull(updateInput, "updateInput");

            var organisationId = "organisations/".AppendWith(updateInput.Id);

            if (!_userContext.HasGroupPermission<Organisation>(PermissionNames.UpdateOrganisation, organisationId))
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
            Check.RequireNotNull(idInput, "idInput");

            var organisationId = "organisations/".AppendWith(idInput.Id);

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
                    Id = organisationId,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        #endregion
    }
}