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
using Bowerbird.Core.Infrastructure;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DomainModels;
using System;

namespace Bowerbird.Web.Controllers
{
    public class UsersController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly IProjectViewModelBuilder _projectViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly ITeamViewModelBuilder _teamViewModelBuilder;
        private readonly IPermissionManager _permissionManager;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;

        #endregion

        #region Constructors

        public UsersController(
            IMessageBus messageBus,
            IUserContext userContext,
            IUserViewModelBuilder userViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            IProjectViewModelBuilder projectViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder,
            ITeamViewModelBuilder teamViewModelBuilder,
            IPermissionManager permissionManager,
            ISightingViewModelBuilder sightingViewModelBuilder)
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(projectViewModelBuilder, "projectViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(teamViewModelBuilder, "teamViewModelBuilder");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");

            _messageBus = messageBus;
            _userContext = userContext;
            _userViewModelBuilder = userViewModelBuilder;
            _projectViewModelBuilder = projectViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _teamViewModelBuilder = teamViewModelBuilder;
            _permissionManager = permissionManager;
            _sightingViewModelBuilder = sightingViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Activity(string id, ActivityInput activityInput, PagingInput pagingInput)
        {
            var userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Activities = _activityViewModelBuilder.BuildUserActivityList(id, activityInput, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "users",
                "activity");
        }

        [HttpGet]
        public ActionResult Sightings(string id, PagingInput pagingInput)
        {
            var userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            var viewModel = new 
            {
                Sightings = _sightingViewModelBuilder.BuildUserSightingList(userId, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "users",
                "sightings");
        }

        [HttpGet]
        public ActionResult Posts(string id, PagingInput pagingInput)
        {
            var userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Model = new
                {
                    Posts = _postViewModelBuilder.BuildUserPostList(userId, pagingInput)
                }
            };

            return RestfulResult(
                viewModel,
                "users",
                "posts");
        }

        [HttpGet]
        public ActionResult About(string id)
        {
            var userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            var userId = VerbosifyId<User>(id);

            if (!_permissionManager.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            var viewModel = new
            {
                Model = new
                {
                    User = _userViewModelBuilder.BuildUser(userId)
                }
            };

            return RestfulResult(
                viewModel,
                "users",
                "index");
        }

        #endregion
    }
}