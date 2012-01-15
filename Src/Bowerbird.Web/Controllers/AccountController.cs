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

using System;
using Bowerbird.Core.Commands;

namespace Bowerbird.Web.Controllers
{
    #region Namespaces

    using System.Web.Mvc;

    using Bowerbird.Core;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.Config;
    using Bowerbird.Core.Tasks;
    using Bowerbird.Core.CommandHandlers;

    #endregion

    public class AccountController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IViewModelRepository _viewModelRepository;
        private readonly IUserTasks _userTasks;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public AccountController(
            ICommandProcessor commandProcessor,
            IViewModelRepository viewModelRepository,
            IUserTasks userTasks,
            IUserContext userContext)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(viewModelRepository, "viewModelRepository");
            Check.RequireNotNull(userTasks, "userTasks");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _viewModelRepository = viewModelRepository;
            _userTasks = userTasks;
            _userContext = userContext;
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

            return View(_viewModelRepository.Load<AccountLogin>());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            if (_userTasks.AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password))
            {
                _commandProcessor.Process(MakeUserUpdateLastLoginCommand(accountLoginInput));

                _userContext.SignUserIn(accountLoginInput.Email, accountLoginInput.RememberMe);

                return RedirectToAction("loggingin", new { returnUrl = accountLoginInput.ReturnUrl });
            }

            return View(_viewModelRepository.Load<AccountLoginInput, AccountLogin>(accountLoginInput));
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

            return RedirectToAction("index", "home");
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
            return View(_viewModelRepository.Load<DefaultViewModel>());
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(_viewModelRepository.Load<AccountRegister>());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Register(AccountRegisterInput accountRegisterInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserCreateCommand(accountRegisterInput));

                return RedirectToAction("registersuccess", "account");
            }

            return View(_viewModelRepository.Load<AccountRegisterInput, AccountRegister>(accountRegisterInput));
        }

        public ActionResult RegisterSuccess()
        {
            return View(_viewModelRepository.Load<DefaultViewModel>());
        }

        [HttpGet]
        public ActionResult RequestPasswordReset()
        {
            return View(_viewModelRepository.Load<AccountRequestPasswordReset>());
        }

        [HttpPost]
        public ActionResult RequestPasswordReset(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            if(ModelState.IsValid)
            {
                if (_userTasks.EmailExists(accountRequestPasswordResetInput.Email))
                {
                    throw new NotImplementedException();
                }

                ModelState.AddModelError("something", "The specified email could not be found. Please check the email and try again.");
            }

            return View(_viewModelRepository.Load<AccountRequestPasswordResetInput, AccountRequestPasswordReset>(accountRequestPasswordResetInput));
        }

        [HttpGet]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput)
        {
            if (!_userTasks.ResetPasswordKeyExists(accountResetPasswordInput.ResetPasswordKey))
            {
                ModelState.AddModelError("something", "The password reset is not valid. Please try again.");
            }

            return View(_viewModelRepository.Load<AccountResetPasswordInput, AccountResetPassword>(accountResetPasswordInput));
        }

        [HttpPost]
        public ActionResult SubmitResetPassword(AccountResetPasswordInput accountResetPasswordInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserUpdatePasswordCommand(accountResetPasswordInput));

                return RedirectToAction("index", "home");
            }

            return View("ResetPassword", _viewModelRepository.Load<AccountResetPasswordInput, AccountResetPassword>(accountResetPasswordInput));
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View(_viewModelRepository.Load<DefaultViewModel>());
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserUpdatePasswordCommand(accountChangePasswordInput));

                return RedirectToAction("index", "home");
            }

            return View(_viewModelRepository.Load<DefaultViewModel>());
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
                Password = accountRegisterInput.Password
            };
        }

        private UserUpdatePasswordCommand MakeUserUpdatePasswordCommand(AccountResetPasswordInput accountResetPasswordInput)
        {
            return new UserUpdatePasswordCommand()
            {
                ResetPasswordKey = accountResetPasswordInput.ResetPasswordKey,
                Password = accountResetPasswordInput.Password
            };
        }

        private UserUpdatePasswordCommand MakeUserUpdatePasswordCommand(AccountChangePasswordInput accountChangePasswordInput)
        {
            return new UserUpdatePasswordCommand()
            {
                UserId = _userContext.GetAuthenticatedUserId(),
                Password = accountChangePasswordInput.Password
            };
        }

        #endregion
    }
}
