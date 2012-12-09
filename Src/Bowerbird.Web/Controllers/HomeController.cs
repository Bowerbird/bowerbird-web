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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.Builders;
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
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public HomeController(
            IMessageBus messageBus,
            IUserContext userContext,
            IActivityViewModelBuilder activityViewModelBuilder,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(documentSession, "documentSession");

            _messageBus = messageBus;
            _userContext = userContext;
            _activityViewModelBuilder = activityViewModelBuilder;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _documentSession = documentSession;
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

            var user = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .Single();

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelBuilder.BuildUser(_userContext.GetAuthenticatedUserId());
            viewModel.Activities = _activityViewModelBuilder.BuildHomeActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput);

            return RestfulResult(
                viewModel,
                "home",
                "privateindex",
                new Action<dynamic>(x =>
                {
                    x.Model.ShowWelcome = user.User.CallsToAction.Contains("welcome");
                    x.Model.ShowActivities = true;
                }));
        }

        /// <summary>
        /// Get a paged list of all the sightings in all projects a user is a member of
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult Sightings(SightingsQueryInput queryInput)
        {
            var user = _documentSession
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
                (queryInput.Sort.ToLower() != "latestadded" && 
                queryInput.Sort.ToLower() != "oldestadded" &&
                queryInput.Sort.ToLower() != "a-z" && 
                queryInput.Sort.ToLower() != "z-a"))
            {
                queryInput.Sort = "latestadded";
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelBuilder.BuildUser(_userContext.GetAuthenticatedUserId());
            viewModel.Sightings = _sightingViewModelBuilder.BuildAllUserProjectsSightingList(_userContext.GetAuthenticatedUserId(), queryInput);
            viewModel.Query = new
                {
                    queryInput.Page,
                    queryInput.PageSize,
                    queryInput.Sort,
                    queryInput.View,
                    IsThumbnailsView = queryInput.View == "thumbnails",
                    IsDetailsView = queryInput.View == "details",
                    IsMapView = queryInput.View == "map"
                };

            return RestfulResult(
                viewModel,
                "home",
                "privateindex",
                new Action<dynamic>(x =>
                {
                    x.Model.ShowWelcome = user.User.CallsToAction.Contains("welcome");
                    x.Model.ShowSightings = true;
                }));
        }

        ///// <summary>
        ///// Get a paged list of all the sightings in all a user's projects
        ///// </summary>
        //[HttpGet]
        //[Authorize]
        //public ActionResult Posts(PagingInput pagingInput)
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        var viewModel = new
        //        {
        //            Sightings = _postViewModelBuilder.BuildAllUserGroupsPostList(_userContext.GetAuthenticatedUserId(), pagingInput)
        //        };

        //        return RestfulResult(
        //            viewModel,
        //            string.Empty,
        //            string.Empty
        //            );
        //    }

        //    return HttpNotFound();
        //}

        #endregion
    }
}