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
    public class OrganisationsController : ControllerBase
    {
        #region Fields

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IOrganisationViewModelBuilder _organisationViewModelBuilder;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly ITeamViewModelBuilder _teamViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IPermissionChecker _permissionChecker;

        #endregion

        #region Constructors

        public OrganisationsController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IOrganisationViewModelBuilder organisationViewModelBuilder,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            ITeamViewModelBuilder teamViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IPermissionChecker permissionChecker
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(organisationViewModelBuilder, "organisationViewModelBuilder");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(teamViewModelBuilder, "teamViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(permissionChecker, "permissionChecker");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _organisationViewModelBuilder = organisationViewModelBuilder;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _teamViewModelBuilder = teamViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _permissionChecker = permissionChecker;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Activity(string id, ActivityInput activityInput, PagingInput pagingInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            var viewModel = _activityViewModelBuilder.BuildGroupActivityList(organisationId, activityInput, pagingInput);

            return RestfulResult(
                viewModel,
                "organisations",
                "activity");
        }

        [HttpGet]
        public ActionResult Sightings(string id, PagingInput pagingInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId),
                Observations = _sightingViewModelBuilder.BuildGroupSightingList(organisationId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "sightings");
        }

        [HttpGet]
        public ActionResult Teams(string id, PagingInput pagingInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId),
                Teams = _teamViewModelBuilder.BuildGroupTeamList(organisationId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "teams");
        }

        [HttpGet]
        public ActionResult Posts(string id, PagingInput pagingInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId),
                Posts = _postViewModelBuilder.BuildGroupPostList(organisationId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "posts");
        }

        [HttpGet]
        public ActionResult Members(string id, PagingInput pagingInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId),
                Members = _userViewModelBuilder.BuildGroupUserList(organisationId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "members");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "index");
        }

        [HttpGet]
        public ActionResult List(PagingInput pagingInput)
        {
            var viewModel = new
            {
                Organisations = _organisationViewModelBuilder.BuildOrganisationList(pagingInput)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "list");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            if (!_userContext.HasAppRootPermission(PermissionNames.CreateOrganisation))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildNewOrganisation()
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "create", 
                new Action<dynamic>(x => x.Model.Create = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateOrganisation, organisationId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "update", 
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasAppRootPermission(PermissionNames.DeleteOrganisation))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId)
            };

            return RestfulResult(
                viewModel,
                "organisations",
                "delete", 
                new Action<dynamic>(x => x.Model.Delete = true));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            // TODO: Not sure what this permission check is actually checking???
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
        public ActionResult Leave(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            // TODO: Not sure what this permission check is actually checking???
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
                    GroupId = organisationId
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
            string organisationId = VerbosifyId<Organisation>(updateInput.Id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

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
        public ActionResult Delete(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionChecker.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

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