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
using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Web.Builders;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Controllers
{
    public class AccountController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IAccountViewModelBuilder _accountViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;

        #endregion

        #region Constructors

        public AccountController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IAccountViewModelBuilder accountViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(accountViewModelBuilder, "accountViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _accountViewModelBuilder = accountViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
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
                return RedirectToAction("PrivateIndex", "home");
            }

            ViewBag.AccountLogin = _accountViewModelBuilder.MakeAccountLogin();
            ViewBag.IsStaticLayout = true;

            return View(Form.Login);
        }

        /// <summary>
        /// TODO: API - detect a JSON login and pass back a cookie/authToken..
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            if (ModelState.IsValid &&
                AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password))
            {
                _commandProcessor.Process(
                    new UserUpdateLastLoginCommand()
                    {
                        Email = accountLoginInput.Email
                    });

                _userContext.SignUserIn(accountLoginInput.Email, accountLoginInput.RememberMe);

#if !JS_COMBINE_MINIFY
    DebugToClient("SERVER: Logged In Successfully as " + accountLoginInput.Email);
#endif

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

            return RedirectToAction("PrivateIndex", "home");
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
                _commandProcessor.Process(
                    new UserCreateCommand()
                    {
                        FirstName = accountRegisterInput.FirstName,
                        LastName = accountRegisterInput.LastName,
                        Email = accountRegisterInput.Email,
                        Password = accountRegisterInput.Password,
                        Roles = new[] { "roles/globalmember" }
                    });

                // HACK: Persist user before _userContext.SignUserIn(..)
                _documentSession.SaveChanges();

                _userContext.SignUserIn(accountRegisterInput.Email.ToLower(), false);

                return RedirectToAction("PrivateIndex", "home");
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
                _commandProcessor.Process(
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
                var email = _documentSession.LoadUserByResetPasswordKey(accountResetPasswordInput.ResetPasswordKey).Email;

                _commandProcessor.Process(
                    new UserUpdatePasswordCommand()
                    {
                        UserId = _documentSession.LoadUserByResetPasswordKey(accountResetPasswordInput.ResetPasswordKey).Id,
                        Password = accountChangePasswordInput.Password
                    });

                _userContext.SignUserIn(email, false);

                return RedirectToAction("PrivateIndex", "home");
            }

            ViewBag.ResetPassword = _accountViewModelBuilder.MakeAccountResetPassword(accountResetPasswordInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.ResetPassword);
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
                _commandProcessor.Process(
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
                _commandProcessor.Process(
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

        private bool AreCredentialsValid(string email, string password)
        {
            var user = _documentSession.LoadUserByEmail(email);

            return user != null && user.ValidatePassword(password);
        }

        #endregion
    }
}