/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Queries;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.Config;
using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;

namespace Bowerbird.Web.Controllers
{
    public class UsersController : ControllerBase
    {
        #region Fields

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IProjectViewModelQuery _projectViewModelQuery;
        private readonly IActivityViewModelQuery _activityViewModelQuery;
        private readonly IPostViewModelQuery _postViewModelQuery;
        private readonly ISightingViewModelQuery _sightingViewModelQuery;
        private readonly IUserViewModelQuery _userViewModelQuery;
        private readonly IPermissionManager _permissionManager;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UsersController(
            IMessageBus messageBus,
            IUserContext userContext,
            IProjectViewModelQuery projectViewModelQuery,
            ISightingViewModelQuery sightingViewModelQuery,
            IActivityViewModelQuery activityViewModelQuery,
            IPostViewModelQuery postViewModelQuery,
            IUserViewModelQuery userViewModelQuery,
            IPermissionManager permissionManager,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(projectViewModelQuery, "projectViewModelQuery");
            Check.RequireNotNull(sightingViewModelQuery, "sightingViewModelQuery");
            Check.RequireNotNull(activityViewModelQuery, "activityViewModelQuery");
            Check.RequireNotNull(postViewModelQuery, "postViewModelQuery");
            Check.RequireNotNull(userViewModelQuery, "userViewModelQuery");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(documentSession, "documentSession");

            _messageBus = messageBus;
            _userContext = userContext;
            _projectViewModelQuery = projectViewModelQuery;
            _sightingViewModelQuery = sightingViewModelQuery;
            _activityViewModelQuery = activityViewModelQuery;
            _postViewModelQuery = postViewModelQuery;
            _userViewModelQuery = userViewModelQuery;
            _permissionManager = permissionManager;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Sightings(string id, SightingsQueryInput queryInput)
        {
            string userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            if (queryInput.View.ToLower() == "thumbnails")
            {
                queryInput.PageSize = 15;
            }

            if (queryInput.View.ToLower() == "details")
            {
                queryInput.PageSize = 10;
            }

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "newest";
            }

            queryInput.Category = queryInput.Category ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(queryInput.Category) && !Categories.IsValidCategory(queryInput.Category))
            {
                queryInput.Category = string.Empty;
            }

            queryInput.Query = queryInput.Query ?? string.Empty;
            queryInput.Field = queryInput.Field ?? string.Empty;

            queryInput.Taxonomy = queryInput.Taxonomy ?? string.Empty;

            var userResult = _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.UserId == userId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(userId);
            viewModel.Sightings = _sightingViewModelQuery.BuildGroupSightingList(userResult.User.UserProject.Id, queryInput);
            viewModel.SightingCountDescription = "Sighting" + (userResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.ProjectCountDescription = "Project" + (userResult.Projects.Count() == 1 ? string.Empty : "s");
            viewModel.OrganisationCountDescription = "Organisation" + (userResult.Organisations.Count() == 1 ? string.Empty : "s");
            viewModel.CategorySelectList = Categories.GetSelectList(queryInput.Category);
            viewModel.Query = new
            {
                Id = userId,
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.View,
                queryInput.Category,
                queryInput.NeedsId,
                queryInput.Query,
                queryInput.Field,
                queryInput.Taxonomy,
                IsThumbnailsView = queryInput.View == "thumbnails",
                IsDetailsView = queryInput.View == "details",
                IsMapView = queryInput.View == "map"
            };
            viewModel.ShowSightings = true;
            viewModel.FieldSelectList = new[]
                {
                    new
                        {
                            Text = "Sighting Title",
                            Value = "title",
                            Selected = queryInput.Field.ToLower() == "title"
                        },
                    new
                        {
                            Text = "Descriptions",
                            Value = "descriptions",
                            Selected = queryInput.Field.ToLower() == "descriptions"
                        },
                    new
                        {
                            Text = "Tags",
                            Value = "tags",
                            Selected = queryInput.Field.ToLower() == "tags"
                        },
                    new
                        {
                            Text = "Scientific Name",
                            Value = "scientificname",
                            Selected = queryInput.Field.ToLower() == "scientificname"
                        },
                    new
                        {
                            Text = "Common Name",
                            Value = "commonname",
                            Selected = queryInput.Field.ToLower() == "commonname"
                        }
                };

            return RestfulResult(
                viewModel,
                "users",
                "sightings");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            string userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            var userResult = _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.UserId == userId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(userId, true);
            viewModel.ShowAbout = true;
            //viewModel.IsMember = _userContext.IsUserAuthenticated() ? _userContext.HasGroupPermission<UserPro>(PermissionNames.CreateObservation, userId) : false;
            viewModel.SightingCountDescription = "Sighting" + (userResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.ProjectCountDescription = "Project" + (userResult.Projects.Count() == 1 ? string.Empty : "s");
            viewModel.OrganisationCountDescription = "Organisation" + (userResult.Organisations.Count() == 1 ? string.Empty : "s");
            viewModel.ActivityTimeseries = CreateActivityTimeseries(userId);

            return RestfulResult(
                viewModel,
                "users",
                "about");
        }

        [HttpGet]
        public ActionResult Index(string id, ActivitiesQueryInput activityInput, PagingInput pagingInput)
        {
            string userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            var userResult = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == userId)
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(userId);
            //viewModel.Activities = _activityViewModelQuery.BuildGroupActivityList(userResult.User.UserProject.Id, activityInput, pagingInput);
            viewModel.Activities = _activityViewModelQuery.BuildUserActivityList(userResult.User.Id, activityInput, pagingInput);
            //viewModel.IsMember = _userContext.IsUserAuthenticated() ? _userContext.HasGroupPermission<Project>(PermissionNames.CreateObservation, projectId) : false;
            viewModel.SightingCountDescription = "Sighting" + (userResult.SightingCount == 1 ? string.Empty : "s");
            viewModel.ProjectCountDescription = "Project" + (userResult.Projects.Count() == 1 ? string.Empty : "s");
            viewModel.OrganisationCountDescription = "Organisation" + (userResult.Organisations.Count() == 1 ? string.Empty : "s");
            viewModel.ShowActivities = true;

            return RestfulResult(
                viewModel,
                "users",
                "index");
        }

        [HttpGet]
        public ActionResult List(UsersQueryInput queryInput)
        {
            queryInput.PageSize = 15;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "a-z";
            }

            queryInput.Query = queryInput.Query ?? string.Empty;
            queryInput.Field = queryInput.Field ?? string.Empty;

            dynamic viewModel = new ExpandoObject();
            viewModel.Users = _userViewModelQuery.BuildUserList(queryInput);
            viewModel.Query = new
            {
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.Query,
                queryInput.Field
            };
            viewModel.FieldSelectList = new[]
                {
                    new
                        {
                            Text = "Name",
                            Value = "name",
                            Selected = queryInput.Field.ToLower() == "name"
                        },
                    new
                        {
                            Text = "Description",
                            Value = "description",
                            Selected = queryInput.Field.ToLower() == "description"
                        }
                };

            return RestfulResult(
                viewModel,
                "users",
                "list");
        }

        private dynamic CreateActivityTimeseries(string projectId)
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)).Date;
            var toDate = DateTime.UtcNow.Date;

            var result = _documentSession
                .Advanced
                .LuceneQuery<All_Contributions.Result, All_Contributions>()
                .SelectFields<All_Contributions.Result>("ParentContributionId", "SubContributionId", "ParentContributionType", "SubContributionType", "CreatedDateTime")
                .WhereGreaterThan(x => x.CreatedDateTime, fromDate)
                .AndAlso()
                .WhereIn("GroupIds", new[] { projectId })
                .AndAlso()
                .OpenSubclause()
                .WhereIn("ParentContributionType", new[] { "observation", "record", "post" })
                .OrElse()
                .WhereIn("SubContributionType", new[] { "identification", "note", "comment" })
                .CloseSubclause()
                .ToList();

            var contributions = result.Select(x => new
            {
                x.ParentContributionId,
                x.SubContributionId,
                x.ParentContributionType,
                x.SubContributionType,
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
                            SightingCount = x.Count(y => y.ParentContributionType == "observation" || y.ParentContributionType == "record"),
                            IdentificationCount = x.Count(y => y.SubContributionType == "identification"),
                            NoteCount = x.Count(y => y.SubContributionType == "note"),
                            PostCount = x.Count(y => y.ParentContributionType == "post"),
                            CommentCount = x.Count(y => y.SubContributionType == "comment")
                        }
                        ).First());
                }
                else
                {
                    timeseries.Add(new
                    {
                        CreatedDate = dateItem.ToString(createdDateFormat),
                        SightingCount = 0,
                        NoteCount = 0,
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