/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Organisation Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;
using System;
using System.Linq;
using System.Collections;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Controllers
{
    public class OrganisationsController : ControllerBase
    {
        #region Fields

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IOrganisationViewModelBuilder _organisationViewModelBuilder;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IPermissionManager _permissionManager;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationsController(
            IMessageBus messageBus,
            IUserContext userContext,
            IOrganisationViewModelBuilder organisationViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IPermissionManager permissionManager,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(organisationViewModelBuilder, "organisationViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(documentSession, "documentSession");

            _messageBus = messageBus;
            _userContext = userContext;
            _organisationViewModelBuilder = organisationViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _permissionManager = permissionManager;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Posts(string id, PagingInput pagingInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
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
        public ActionResult Members(string id, UsersQueryInput queryInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            queryInput.PageSize = 15;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "a-z";
            }

            dynamic organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId);
            viewModel.Members = _userViewModelBuilder.BuildGroupUserList(organisationId, queryInput);
            viewModel.Query = new
            {
                Id = organisationId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort
            };
            viewModel.ShowMembers = true;
            viewModel.IsMember = _userContext.HasGroupPermission<Organisation>(PermissionNames.CreateObservation, organisationId);
            viewModel.MemberCountDescription = "Member" + (organisation.MemberCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (organisation.PostCount == 1 ? string.Empty : "s");

            return RestfulResult(
                viewModel,
                "organisations",
                "members");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            dynamic organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId);
            viewModel.ShowAbout = true;
            viewModel.IsMember = _userContext.HasGroupPermission<Organisation>(PermissionNames.CreateObservation, organisationId);
            viewModel.MemberCountDescription = "Member" + (organisation.MemberCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (organisation.PostCount == 1 ? string.Empty : "s");
            viewModel.OrganisationAdministrators = _userViewModelBuilder.BuildGroupUserList(organisationId, "roles/" + RoleNames.OrganisationAdministrator);
            viewModel.ActivityTimeseries = CreateActivityTimeseries(organisationId);

            return RestfulResult(
                viewModel,
                "organisations",
                "about");
        }

        [HttpGet]
        public ActionResult Index(string id, ActivityInput activityInput, PagingInput pagingInput)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            dynamic organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId);

            dynamic viewModel = new ExpandoObject();
            viewModel.Organisation = organisation;
            viewModel.Activities = _activityViewModelBuilder.BuildGroupActivityList(organisationId, activityInput, pagingInput);
            viewModel.IsMember = _userContext.HasGroupPermission<Organisation>(PermissionNames.CreateObservation, organisationId);
            viewModel.MemberCountDescription = "Member" + (organisation.MemberCount == 1 ? string.Empty : "s");
            viewModel.PostCountDescription = "Post" + (organisation.PostCount == 1 ? string.Empty : "s");
            viewModel.ShowActivities = true;

            return RestfulResult(
                viewModel,
                "organisations",
                "index");
        }

        [HttpGet]
        public ActionResult List(OrganisationsQueryInput queryInput)
        {
            queryInput.PageSize = 15;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" && 
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "newest";
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Organisations = _organisationViewModelBuilder.BuildOrganisationList(queryInput);
            viewModel.Query = new
            {
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort
            };

            if (_userContext.IsUserAuthenticated())
            {
                var user = _documentSession
                    .Query<All_Users.Result, All_Users>()
                    .AsProjection<All_Users.Result>()
                    .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                    .Single();

                viewModel.ShowOrganisationExploreWelcome = user.User.CallsToAction.Contains("organisation-explore-welcome");
            }

            return RestfulResult(
                viewModel,
                "organisations",
                "list");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateForm()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.Organisation = _organisationViewModelBuilder.BuildCreateOrganisation();
            viewModel.Create = true;

            return RestfulResult(
                viewModel,
                "organisations",
                "create");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            if (!_userContext.HasGroupPermission(PermissionNames.UpdateOrganisation, organisationId))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Organisation = _organisationViewModelBuilder.BuildUpdateOrganisation(organisationId);
            viewModel.Update = true;

            return RestfulResult(
                viewModel,
                "organisations",
                "update");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteOrganisation, organisationId))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Organisation = _organisationViewModelBuilder.BuildOrganisation(organisationId);
            viewModel.Delete = true;

            return RestfulResult(
                viewModel,
                "organisations",
                "delete");
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Join(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new MemberCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = organisationId,
                    CreatedByUserId = _userContext.GetAuthenticatedUserId(),
                    Roles = new[] { "roles/organisationmember" }
                });

            return JsonSuccess();
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Leave(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            //// TODO: Not sure what this permission check is actually checking???
            //if (!_userContext.HasGroupPermission(PermissionNames.LeaveOrganisation, organisationId))
            //{
            //    return HttpUnauthorized();
            //}

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new MemberDeleteCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    GroupId = organisationId,
                    ModifiedByUserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPost]
        [Authorize]
        public ActionResult Create(OrganisationCreateInput createInput)
        {
            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new OrganisationCreateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Name = createInput.Name,
                    Description = createInput.Description,
                    Website = createInput.Website,
                    AvatarId = createInput.AvatarId,
                    BackgroundId = createInput.BackgroundId
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpPut]
        [Authorize]
        public ActionResult Update(OrganisationUpdateInput updateInput)
        {
            string organisationId = VerbosifyId<Organisation>(updateInput.Id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
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

            _messageBus.Send(
                new OrganisationUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Id = organisationId,
                    Name = updateInput.Name,
                    Description = updateInput.Description,
                    Website = updateInput.Website,
                    AvatarId = updateInput.AvatarId,
                    BackgroundId = updateInput.BackgroundId
                });

            return JsonSuccess();
        }

        [Transaction]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string id)
        {
            string organisationId = VerbosifyId<Organisation>(id);

            if (!_permissionManager.DoesExist<Organisation>(organisationId))
            {
                return HttpNotFound();
            }

            // BUG: Fix this to check the parent groups' permission
            if (!_userContext.HasGroupPermission(PermissionNames.DeleteOrganisation, organisationId))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new OrganisationDeleteCommand
                {
                    Id = organisationId,
                    UserId = _userContext.GetAuthenticatedUserId()
                });

            return JsonSuccess();
        }

        private dynamic CreateActivityTimeseries(string organisationId)
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)).Date;
            var toDate = DateTime.UtcNow.Date;

            var result = _documentSession
                .Advanced
                .LuceneQuery<All_Contributions.Result, All_Contributions>()
                .SelectFields<All_Contributions.Result>("ContributionId", "SubContributionId", "ContributionType", "CreatedDateTime")
                .WhereGreaterThan(x => x.CreatedDateTime, fromDate)
                .AndAlso()
                .WhereIn("GroupIds", new[] { organisationId })
                .AndAlso()
                .WhereIn("ContributionType", new[] { "post", "comment" })
                .ToList();

            var contributions = result.Select(x => new
            {
                x.ContributionId,
                x.SubContributionId,
                x.ContributionType,
                x.CreatedDateTime
            })
                .GroupBy(x => x.CreatedDateTime.Date);

            var timeseries = new List<dynamic>();

            for (DateTime dateItem = fromDate; dateItem <= toDate; dateItem = dateItem.AddDays(1))
            {
                string createdDateFormat;

                if (dateItem == fromDate ||
                    dateItem.Day == 1)
                {
                    createdDateFormat = "d MMM";
                }
                else
                {
                    createdDateFormat = "%d";
                }

                if (contributions.Any(x => x.Key.Date == dateItem.Date))
                {
                    timeseries.Add(contributions
                        .Where(x => x.Key.Date == dateItem.Date)
                        .Select(x => new
                        {
                            CreatedDate = dateItem.ToString(createdDateFormat),
                            PostCount = x.Count(y => y.ContributionType == "post"),
                            CommentCount = x.Count(y => y.ContributionType == "comment")
                        }
                        ).First());
                }
                else
                {
                    timeseries.Add(new
                    {
                        CreatedDate = dateItem.ToString(createdDateFormat),
                        PostCount = 0,
                        CommentCount = 0
                    });
                }
            }
            return timeseries;
        }

        #endregion
    }
}