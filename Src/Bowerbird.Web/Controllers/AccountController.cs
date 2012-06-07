/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Web.Builders;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class AccountController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IAccountViewModelBuilder _accountViewModelBuilder;

        #endregion

        #region Constructors

        public AccountController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IAccountViewModelBuilder accountViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(accountViewModelBuilder, "accountViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _accountViewModelBuilder = accountViewModelBuilder;
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

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            if (ModelState.IsValid &&
                _accountViewModelBuilder.AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password))
            {
                _commandProcessor.Process(
                    new UserUpdateLastLoginCommand()
                    {
                        Email = accountLoginInput.Email
                    });

                _userContext.SignUserIn(accountLoginInput.Email, accountLoginInput.RememberMe);

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

                // persist user before _userContext.SignUserIn(..)
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

        #endregion
    }
}