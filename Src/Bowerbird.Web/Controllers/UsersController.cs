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

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly IProjectViewModelBuilder _projectViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;
        private readonly ITeamViewModelBuilder _teamViewModelBuilder;
        private readonly IPermissionChecker _permissionChecker;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;

        #endregion

        #region Constructors

        public UsersController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IUserViewModelBuilder userViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            IProjectViewModelBuilder projectViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder,
            ITeamViewModelBuilder teamViewModelBuilder,
            IPermissionChecker permissionChecker,
            ISightingViewModelBuilder sightingViewModelBuilder)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(projectViewModelBuilder, "projectViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");
            Check.RequireNotNull(teamViewModelBuilder, "teamViewModelBuilder");
            Check.RequireNotNull(permissionChecker, "permissionChecker");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _userViewModelBuilder = userViewModelBuilder;
            _projectViewModelBuilder = projectViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _teamViewModelBuilder = teamViewModelBuilder;
            _permissionChecker = permissionChecker;
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

            if (!_permissionChecker.DoesExist<User>(userId))
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

            if (!_permissionChecker.DoesExist<User>(userId))
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

            if (!_permissionChecker.DoesExist<User>(userId))
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

            if (!_permissionChecker.DoesExist<User>(userId))
            {
                return HttpNotFound();
            }

            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            var userId = VerbosifyId<User>(id);

            if (!_permissionChecker.DoesExist<User>(userId))
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

//        [HttpGet]
//        [Authorize]
//        public ActionResult UpdateForm(IdInput idInput)
//        {
//            Check.RequireNotNull(idInput, "idInput");

//            var userId = idInput.Id.VerbosifyId<User>();

//#if !JS_COMBINE_MINIFY
//    DebugToClient("SERVER: Users/UpdateForm userId:" + userId);
//#endif

//            ViewBag.Model = new
//            {
//                User = _userViewModelBuilder.BuildEditableUser(userId)
//            };

//            if (Request.IsAjaxRequest())
//            {
//                return new JsonNetResult(new
//                {
//                    Model = ViewBag.Model
//                });
//            }

//            ViewBag.PrerenderedView = "users";

//            return View(Form.Update);
//        }

        ///// <summary>
        ///// Placeholder Method: Keeping Restful Convention
        ///// </summary>
        //[HttpGet]
        //[Authorize]
        //public ActionResult DeleteForm(IdInput idInput)
        //{
        //    Check.RequireNotNull(idInput, "idInput");

        //    var userId = idInput.Id.VerbosifyId<User>();

        //    ViewBag.User = _userViewModelBuilder.BuildUser(userId);

        //    return View(Form.Delete);
        //}

        //[HttpPut]
        //[Authorize]
        //[Transaction]
        //public ActionResult Update(UserUpdateInput userUpdateInput)
        //{
        //    Check.RequireNotNull(userUpdateInput, "userUpdateInput");

        //    var userId = userUpdateInput.Id.VerbosifyId<User>();

        //    if (!_userContext.HasUserPermission(userId))
        //    {
        //        return HttpUnauthorized();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _commandProcessor.Process(
        //            new UserUpdateCommand()
        //            {
        //                Id = userUpdateInput.Id,
        //                FirstName = userUpdateInput.FirstName,
        //                LastName = userUpdateInput.LastName,
        //                Email = userUpdateInput.Email,
        //                Description = userUpdateInput.Description,
        //                AvatarId = userUpdateInput.AvatarId
        //            });

        //        return RedirectToAction("index", "home");
        //    }

        //    ViewBag.User = new
        //    {
        //        userUpdateInput.Id,
        //        userUpdateInput.AvatarId,
        //        userUpdateInput.Description,
        //        userUpdateInput.Email,
        //        userUpdateInput.FirstName,
        //        userUpdateInput.LastName
        //    };

        //    return View(Form.Update);
        //}

        ///// <summary>
        ///// Placeholder Method: Keeping Restful Convention
        ///// </summary>
        //[HttpDelete]
        //[Authorize]
        //public ActionResult Delete()
        //{
        //    return HttpNotFound();
        //}

        //[HttpGet]
        //[Authorize]
        //public ActionResult ChangePassword()
        //{
        //    return View(Form.ChangePassword);
        //}

        //[HttpPost]
        //[Authorize]
        //[Transaction]
        //public ActionResult ChangePassword(AccountChangePasswordInput accountChangePasswordInput)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _commandProcessor.Process(
        //            new UserUpdatePasswordCommand()
        //            {
        //                UserId = _userContext.GetAuthenticatedUserId(),
        //                Password = accountChangePasswordInput.Password
        //            });

        //        return RedirectToAction("index", "home");
        //    }

        //    return View(Form.ChangePassword);
        //}

        #endregion
    }
}