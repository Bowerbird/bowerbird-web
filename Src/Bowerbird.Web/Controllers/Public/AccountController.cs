/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Public;
using Raven.Client;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Controllers.Public
{
    public class AccountController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public AccountController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
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
                return RedirectToAction("index", "home");
            }

            return View(MakeAccountLogin());
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            if (ModelState.IsValid &&
                AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password))
            {
                _commandProcessor.Process(MakeUserUpdateLastLoginCommand(accountLoginInput));

                _userContext.SignUserIn(accountLoginInput.Email, accountLoginInput.RememberMe);

                return RedirectToAction("loggingin", new { returnUrl = accountLoginInput.ReturnUrl });
            }
            
            ModelState.AddModelError("", "");

            return View(MakeAccountLogin(accountLoginInput));
        }

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

            return RedirectToAction("index", "home", new { area = "members" });
        }

        public ActionResult Logout()
        {
            _userContext.SignUserOut();

            // Even though we have signed out via FormsAuthentication, the session still contains the User.Identity until the 
            // HTTP response is fully written. In order for the User.Identity to be properly cleared, we have to do a full HTTP redirect 
            // rather than simply showing of the View in this action.
            return RedirectToAction("logoutsuccess");
        }

        public ActionResult LogoutSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(MakeAccountRegister());
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Register(AccountRegisterInput accountRegisterInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserCreateCommand(accountRegisterInput));

                _userContext.SignUserIn(accountRegisterInput.Email.ToLower(), false);

                return RedirectToAction("index", "home");
            }

            return View(MakeAccountRegister(accountRegisterInput));
        }

        [HttpGet]
        public ActionResult RequestPasswordReset()
        {
            return View(MakeAccountRequestPasswordReset());
        }

        [HttpPost]
        [Transaction]
        public ActionResult RequestPasswordReset(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            if(ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserRequestPasswordResetCommand(accountRequestPasswordResetInput));

                return RedirectToAction("requestpasswordresetsuccess", "account");
            }

            return View(MakeAccountRequestPasswordReset(accountRequestPasswordResetInput));
        }

        public ActionResult RequestPasswordResetSuccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput)
        {
            return View(MakeAccountResetPassword(accountResetPasswordInput));
        }

        [HttpPost]
        [Transaction]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput, AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                string email = _documentSession.LoadUserByResetPasswordKey(accountResetPasswordInput.ResetPasswordKey).Email;

                _commandProcessor.Process(MakeUserUpdatePasswordCommand(accountResetPasswordInput, accountChangePasswordInput));

                _userContext.SignUserIn(email, false);

                return RedirectToAction("index", "home");
            }

            return View(MakeAccountResetPassword(accountResetPasswordInput));
        }

        private UserUpdateLastLoginCommand MakeUserUpdateLastLoginCommand(AccountLoginInput accountLoginInput)
        {
            return new UserUpdateLastLoginCommand()
            {
                Email = accountLoginInput.Email
            };
        }

        private UserCreateCommand MakeUserCreateCommand(AccountRegisterInput accountRegisterInput)
        {
            return new UserCreateCommand()
            {
                FirstName = accountRegisterInput.FirstName,
                LastName = accountRegisterInput.LastName,
                Email = accountRegisterInput.Email,
                Password = accountRegisterInput.Password,
                Roles = new [] { "globalmember" }
            };
        }

        private UserUpdatePasswordCommand MakeUserUpdatePasswordCommand(AccountResetPasswordInput accountResetPasswordInput, AccountChangePasswordInput accountChangePasswordInput)
        {
            return new UserUpdatePasswordCommand()
                       {
                           UserId = _documentSession.LoadUserByResetPasswordKey(accountResetPasswordInput.ResetPasswordKey).Id,
                           Password = accountChangePasswordInput.Password
                       };
        }

        private UserRequestPasswordResetCommand MakeUserRequestPasswordResetCommand(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            return new UserRequestPasswordResetCommand()
                       {
                           Email = accountRequestPasswordResetInput.Email
                       };
        }

        private AccountLogin MakeAccountLogin()
        {
            return new AccountLogin()
            {
                Email = _userContext.HasEmailCookieValue() ? _userContext.GetEmailCookieValue() : string.Empty
            };
        }

        private AccountLogin MakeAccountLogin(AccountLoginInput accountLoginInput)
        {
            return new AccountLogin()
            {
                Email = accountLoginInput.Email,
                RememberMe = accountLoginInput.RememberMe,
                ReturnUrl = accountLoginInput.ReturnUrl
            };
        }

        private AccountRegister MakeAccountRegister()
        {
            return new AccountRegister();
        }

        private AccountRegister MakeAccountRegister(AccountRegisterInput accountRegisterInput)
        {
            return new AccountRegister()
            {
                FirstName = accountRegisterInput.FirstName,
                LastName = accountRegisterInput.LastName,
                Email = accountRegisterInput.Email
            };
        }

        private AccountRequestPasswordReset MakeAccountRequestPasswordReset()
        {
            return new AccountRequestPasswordReset();
        }

        private AccountRequestPasswordReset MakeAccountRequestPasswordReset(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            return new AccountRequestPasswordReset()
            {
                Email = accountRequestPasswordResetInput.Email
            };
        }

        private AccountResetPassword MakeAccountResetPassword(AccountResetPasswordInput accountResetPasswordInput)
        {
            return new AccountResetPassword()
            {
                ResetPasswordKey = accountResetPasswordInput.ResetPasswordKey
            };
        }

        private bool AreCredentialsValid(string email, string password)
        {
            var user = _documentSession.LoadUserByEmail(email);

            return user != null && user.ValidatePassword(password);
        }

        #endregion
    }
}
