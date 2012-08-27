/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Dynamic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Repositories;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class AccountController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IAccountViewModelBuilder _accountViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;

        #endregion

        #region Constructors

        public AccountController(
            IMessageBus messageBus,
            IUserContext userContext,
            IDocumentSession documentSession,
            IAccountViewModelBuilder accountViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            IUserViewFactory userViewFactory,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(accountViewModelBuilder, "accountViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");

            _messageBus = messageBus;
            _userContext = userContext;
            _documentSession = documentSession;
            _accountViewModelBuilder = accountViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _userViewFactory = userViewFactory;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Login()
        {
            if (_userContext.IsUserAuthenticated())
            {
                return RedirectToAction("privateindex", "home");
            }

            ViewBag.AccountLogin = _accountViewModelBuilder.MakeAccountLogin();
            ViewBag.IsStaticLayout = true;

            return View(Form.Login);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            User user = null;

            if (ModelState.IsValid &&
                AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password, out user))
            {
                _messageBus.Send(
                    new UserUpdateLastLoginCommand()
                    {
                        Email = accountLoginInput.Email
                    });
                
                _userContext.SignUserIn(user.Id, user.Email, accountLoginInput.RememberMe);

#if !JS_COMBINE_MINIFY
    DebugToClient("SERVER: Logged In Successfully as " + accountLoginInput.Email);
#endif

                if(Request.IsAjaxRequest())
                {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.User = _userViewFactory.Make(user);

                    return RestfulResult(
                        viewModel,
                        string.Empty,
                        string.Empty,
                        null,
                        null);
                }

                return RedirectToAction("loggingin", new { returnUrl = accountLoginInput.ReturnUrl });
            }

            ModelState.AddModelError("", "");

            ViewBag.AccountLogin = _accountViewModelBuilder.MakeAccountLogin(accountLoginInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.Login);
        }

        [HttpGet]
        public ActionResult LoggingIn(string returnUrl)
        {
            if (!_userContext.HasEmailCookieValue())
            {
                // User attempted to login without cookies enabled
                return RedirectToAction("login");
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _userContext.SignUserOut();

            // Even though we have signed out via FormsAuthentication, the session still contains the User.Identity until the 
            // HTTP response is fully written. In order for the User.Identity to be properly cleared, we have to do a full HTTP redirect 
            // rather than simply showing of the View in this action.
            return RedirectToAction("logoutsuccess");
        }

        [HttpGet]
        public ActionResult LogoutSuccess()
        {
            ViewBag.IsStaticLayout = true;

            return View(Form.LogoutSuccess);
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.IsStaticLayout = true;

            return View(Form.Register);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Register(AccountRegisterInput accountRegisterInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserCreateCommand()
                    {
                        FirstName = accountRegisterInput.FirstName,
                        LastName = accountRegisterInput.LastName,
                        Email = accountRegisterInput.Email,
                        Password = accountRegisterInput.Password,
                        DefaultLicence = Constants.DefaultLicence,
                        Timezone = Constants.DefaultTimezone,
                        Roles = new[] { "roles/globalmember" }
                    });

                // HACK: Persist user before _userContext.SignUserIn(..)
                _documentSession.SaveChanges();

                // HACK: Wait a couple of seconds to ensure all indexes are up to date
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds < 3000) {}

                User user = _documentSession.LoadUserByEmail(accountRegisterInput.Email);

                _userContext.SignUserIn(user.Id, accountRegisterInput.Email.ToLower(), accountRegisterInput.RememberMe);

                // App login
                if (Request.IsAjaxRequest())
                {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.User = _userViewFactory.Make(user);

                    return RestfulResult(
                        viewModel,
                        string.Empty,
                        string.Empty,
                        null,
                        null);
                }

                return RedirectToAction("loggingin");
            }

            ViewBag.AccountRegister = _accountViewModelBuilder.MakeAccountRegister(accountRegisterInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.Register);
        }

        [HttpGet]
        public ActionResult RequestPasswordReset()
        {
            ViewBag.IsStaticLayout = true;

            return View(Form.RequestPasswordReset);
        }

        [HttpPost]
        [Transaction]
        public ActionResult RequestPasswordReset(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserRequestPasswordResetCommand()
                    {
                        Email = accountRequestPasswordResetInput.Email
                    });

                return RedirectToAction("requestpasswordresetsuccess", "account");
            }

            ViewBag.RequestPasswordReset = _accountViewModelBuilder.MakeAccountRequestPasswordReset(accountRequestPasswordResetInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.RequestPasswordReset);
        }

        [HttpGet]
        public ActionResult RequestPasswordResetSuccess()
        {
            return View(Form.RequestPasswordResetSuccess);
        }

        [HttpGet]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput)
        {
            ViewBag.RequestPasswordResetSuccess = _accountViewModelBuilder.MakeAccountResetPassword(accountResetPasswordInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.ResetPassword);
        }

        [HttpPost]
        [Transaction]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput, AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                var user = _documentSession.LoadUserByResetPasswordKey(accountResetPasswordInput.ResetPasswordKey);

                _messageBus.Send(
                    new UserUpdatePasswordCommand()
                    {
                        UserId = user.Id,
                        Password = accountChangePasswordInput.Password
                    });

                _userContext.SignUserIn(user.Id, user.Email, false);

                return RedirectToAction("privateindex", "home");
            }

            ViewBag.ResetPassword = _accountViewModelBuilder.MakeAccountResetPassword(accountResetPasswordInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.ResetPassword);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Profile()
        {
            if (Request.IsAjaxRequest())
            {
                dynamic viewModel = new ExpandoObject();
                viewModel.AuthenticatedUser =
                    _userViewModelBuilder.BuildAuthenticatedUser(_userContext.GetAuthenticatedUserId());

                return RestfulResult(
                    viewModel,
                    string.Empty,
                    string.Empty
                    );
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Get a paged list of all the sightings a user has created
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult MySightings(PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                var viewModel = new
                {
                    Sightings =_sightingViewModelBuilder.BuildUserSightingList(_userContext.GetAuthenticatedUserId(), pagingInput)
                };

                return RestfulResult(
                    viewModel,
                    string.Empty,
                    string.Empty
                    );
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Get a paged list of all the sightings in all a user's projects
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult AllSightings(PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                var viewModel = new
                {
                    Sightings = _sightingViewModelBuilder.BuildAllUserProjectsSightingList(_userContext.GetAuthenticatedUserId(), pagingInput)
                };

                return RestfulResult(
                    viewModel,
                    string.Empty,
                    string.Empty
                    );
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Get a paged list of all the sightings a user has created
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult MyPosts(PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                var viewModel = new
                {
                    Sightings = _postViewModelBuilder.BuildUserPostList(_userContext.GetAuthenticatedUserId(), pagingInput)
                };

                return RestfulResult(
                    viewModel,
                    string.Empty,
                    string.Empty
                    );
            }

            return HttpNotFound();
        }

        /// <summary>
        /// Get a paged list of all the sightings in all a user's projects
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult AllPosts(PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                var viewModel = new
                {
                    Sightings = _postViewModelBuilder.BuildAllUserGroupsPostList(_userContext.GetAuthenticatedUserId(), pagingInput)
                };

                return RestfulResult(
                    viewModel,
                    string.Empty,
                    string.Empty
                    );
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Notifications(ActivityInput activityInput, PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = new
                    {
                        Activities = _activityViewModelBuilder.BuildNotificationActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput)
                    }
                });
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Activity(ActivityInput activityInput, PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = new
                    {
                        Activities = _activityViewModelBuilder.BuildHomeActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput)
                    }
                });
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Update()
        {
            var userId = _userContext.GetAuthenticatedUserId();

            if (!_userContext.HasUserPermission(userId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new
            {
                User = _userViewModelBuilder.BuildEditableUser(userId)
            };

            return RestfulResult(
                viewModel,
                "account",
                "update",
                new Action<dynamic>(x => x.Update = true));
        }

        [HttpPut]
        [Authorize]
        [Transaction]
        public ActionResult Update(UserUpdateInput userUpdateInput)
        {
            var userId = VerbosifyId<User>(userUpdateInput.Id);

            if (!_userContext.HasUserPermission(userId))
            {
                return HttpUnauthorized();
            }

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserUpdateCommand()
                    {
                        Id = userUpdateInput.Id,
                        FirstName = userUpdateInput.FirstName,
                        LastName = userUpdateInput.LastName,
                        Email = userUpdateInput.Email,
                        Description = userUpdateInput.Description,
                        AvatarId = userUpdateInput.AvatarId
                    });

                if (Request.IsAjaxRequest())
                {
                    return JsonSuccess();
                }

                return RedirectToAction("index", "home");
            }

            if (Request.IsAjaxRequest())
            {
                return JsonFailed();
            }
            
            ViewBag.Model.User = new
            {
                userUpdateInput.Id,
                userUpdateInput.AvatarId,
                userUpdateInput.Description,
                userUpdateInput.Email,
                userUpdateInput.FirstName,
                userUpdateInput.LastName
            };

            return View(Form.Update);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            var userId = _userContext.GetAuthenticatedUserId();

            if (!_userContext.HasUserPermission(userId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new {};

            return RestfulResult(
                viewModel,
                "account",
                "changepassword");
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult ChangePassword(AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserUpdatePasswordCommand()
                    {
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Password = accountChangePasswordInput.Password
                    });

                if(Request.IsAjaxRequest())
                {
                    return JsonSuccess();
                }

                return RedirectToAction("index", "home");
            }

            if (Request.IsAjaxRequest())
            {
                return JsonFailed();
            }

            return View(Form.ChangePassword);
        }

        private bool AreCredentialsValid(string email, string password, out User user)
        {
            user = _documentSession.LoadUserByEmail(email);

            return user != null && user.ValidatePassword(password);
        }

        #endregion
    }
}