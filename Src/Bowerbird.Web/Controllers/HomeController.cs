/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.ViewModels;
using Bowerbird.Core.Queries;
using System;
using System.Dynamic;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IActivityViewModelQuery _activityViewModelQuery;
        private readonly ISightingViewModelQuery _sightingViewModelQuery;
        private readonly IUserViewModelQuery _userViewModelQuery;
        private readonly IDocumentSession _documentSession;
        private readonly IPostViewModelQuery _postViewModelQuery;

        #endregion

        #region Constructors

        public HomeController(
            IMessageBus messageBus,
            IUserContext userContext,
            IActivityViewModelQuery activityViewModelQuery,
            ISightingViewModelQuery sightingViewModelQuery,
            IUserViewModelQuery userViewModelQuery,
            IDocumentSession documentSession,
            IPostViewModelQuery postViewModelQuery
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(activityViewModelQuery, "activityViewModelQuery");
            Check.RequireNotNull(sightingViewModelQuery, "sightingViewModelQuery");
            Check.RequireNotNull(userViewModelQuery, "userViewModelQuery");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(postViewModelQuery, "postViewModelQuery");

            _messageBus = messageBus;
            _userContext = userContext;
            _activityViewModelQuery = activityViewModelQuery;
            _sightingViewModelQuery = sightingViewModelQuery;
            _userViewModelQuery = userViewModelQuery;
            _documentSession = documentSession;
            _postViewModelQuery = postViewModelQuery;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult PublicIndex()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.HomeHeader = true;
            
            return RestfulResult(
                viewModel,
                "home",
                "publicindex",
                null);
        }

        [HttpGet]
        [Authorize]
        public ActionResult PrivateIndex(ActivityInput activityInput, PagingInput pagingInput)
        {
            if (!_userContext.IsUserAuthenticated())
            {
                return RedirectToAction("PublicIndex");
            }

            var userResult = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .First();

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());
            viewModel.Activities = _activityViewModelQuery.BuildHomeActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput);
            viewModel.ShowUserWelcome = userResult.User.CallsToAction.Contains("user-welcome");
            viewModel.ShowActivities = true;

            return RestfulResult(
                viewModel,
                "home",
                "privateindex");
        }

        /// <summary>
        /// Get a paged list of all the sightings in all groups a user is a member of
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult Sightings(SightingsQueryInput queryInput)
        {
            var userResult = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .First();

            if (queryInput.View.ToLower() == "thumbnails")
            {
                queryInput.PageSize = 15;
            }

            if (queryInput.View.ToLower() == "details")
            {
                queryInput.PageSize = 10;
            }

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" && 
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

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());
            viewModel.Sightings = _sightingViewModelQuery.BuildHomeSightingList(_userContext.GetAuthenticatedUserId(), queryInput);
            viewModel.CategorySelectList = Categories.GetSelectList(queryInput.Category);
            viewModel.Query = new
                {
                    Id = "home", // We set the id to home, so that the mustache sightings list creates correct sorting URL
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
            viewModel.ShowUserWelcome = userResult.User.CallsToAction.Contains("user-welcome");
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
                "home",
                "sightings");
        }

        /// <summary>
        /// Get a paged list of all the posts in all groups a user is a member of
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult Posts(PostsQueryInput queryInput)
        {
            queryInput.PageSize = 10;

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "newest";
            }

            queryInput.Query = queryInput.Query ?? string.Empty;
            queryInput.Field = queryInput.Field ?? string.Empty;

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());
            viewModel.Posts = _postViewModelQuery.BuildHomePostList(_userContext.GetAuthenticatedUserId(), queryInput);
            viewModel.Query = new
            {
                Id = "home", // We set the id to home, so that the mustache sightings list creates correct sorting URL
                queryInput.Page,
                queryInput.PageSize,
                queryInput.Sort,
                queryInput.Query,
                queryInput.Field
            };
            viewModel.ShowPosts = true;
            viewModel.FieldSelectList = new[]
                {
                    new
                        {
                            Text = "Title",
                            Value = "title",
                            Selected = queryInput.Field.ToLower() == "title"
                        },
                    new
                        {
                            Text = "Body",
                            Value = "descriptions",
                            Selected = queryInput.Field.ToLower() == "descriptions"
                        }
                };

            return RestfulResult(
                viewModel,
                "home",
                "posts");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Favourites(SightingsQueryInput queryInput)
        {
            var userResult = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .Single();

            if (queryInput.View.ToLower() == "thumbnails")
            {
                queryInput.PageSize = 15;
            }

            if (queryInput.View.ToLower() == "details")
            {
                queryInput.PageSize = 10;
            }

            if (string.IsNullOrWhiteSpace(queryInput.Sort) ||
                (queryInput.Sort.ToLower() != "newest" &&
                queryInput.Sort.ToLower() != "oldest" &&
                queryInput.Sort.ToLower() != "a-z" &&
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

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());
            viewModel.Sightings = _sightingViewModelQuery.BuildGroupSightingList(userResult.User.UserProject.Id, queryInput);
            viewModel.CategorySelectList = Categories.GetSelectList(queryInput.Category);
            viewModel.Query = new
            {
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
            viewModel.ShowUserWelcome = userResult.User.CallsToAction.Contains("user-welcome");
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
                "home",
                "favourites");
        }

        #endregion

        #region Static Content Methods

        [HttpGet]
        public ActionResult Blog()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());

            return RestfulResult(
                viewModel,
                "home",
                "blog");
        }

        [HttpGet]
        public ActionResult Privacy()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());

            return RestfulResult(
                viewModel,
                "home",
                "privacy");
        }

        [HttpGet]
        public ActionResult Terms()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());

            return RestfulResult(
                viewModel,
                "home",
                "terms");
        }

        [HttpGet]
        public ActionResult About()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());

            return RestfulResult(
                viewModel,
                "home",
                "about");
        }

        [HttpGet]
        public ActionResult Resources()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());

            return RestfulResult(
                viewModel,
                "home",
                "resources");
        }

        [HttpGet]
        public ActionResult Developer()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelQuery.BuildUser(_userContext.GetAuthenticatedUserId());

            return RestfulResult(
                viewModel,
                "home",
                "developer");
        }

        #endregion
    }
}