using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Entities;
using Bowerbird.Web.Config;
using Bowerbird.Core.Tasks;
using Bowerbird.Core.CommandHandlers;

namespace Bowerbird.Web.Controllers
{
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }

            var accountLoginInput = new AccountLoginInput()
            {
                Username = _userContext.HasUsernameCookieValue() ? _userContext.GetUsernameCookieValue() : string.Empty
            };

            return View(_viewModelRepository.Load<AccountLoginInput, AccountLogin>(accountLoginInput));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            if (_userTasks.AreCredentialsValid(accountLoginInput.Username, accountLoginInput.Password))
            {
                _commandProcessor.Process<UserUpdateLastLoginCommand>(MakeUserUpdateLastLoginCommand(accountLoginInput));

                _userContext.SignUserIn(accountLoginInput.Username, accountLoginInput.RememberMe);

                return RedirectToAction("loggingin", new { returnUrl = accountLoginInput.ReturnUrl });
            }

            return View(_viewModelRepository.Load<AccountLoginInput, AccountLogin>(accountLoginInput));
        }

        public ActionResult LoggingIn(string returnUrl)
        {
            if (!_userContext.HasUsernameCookieValue())
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

        private UserUpdateLastLoginCommand MakeUserUpdateLastLoginCommand(AccountLoginInput accountLoginInput)
        {
            return new UserUpdateLastLoginCommand()
            {
                Username = accountLoginInput.Username
            };
        }

        //[HttpGet]
        //public ActionResult Register()
        //{
        //    throw new NotImplementedException();
        //}

        //[HttpPost]
        //public ActionResult Register(AccountRegisterViewModel accountRegisterViewModel)
        //{
        //    if (_userCreateCommandValidator.IsValid(accountRegisterViewModel))
        //    {
        //        _commandProcessor.Process<IUserCreateCommand>(accountRegisterViewModel);

        //        return RedirectToAction("RegisterSuccess");
        //    }

        //    _accountRegisterViewModelBuilder.Build(accountRegisterViewModel);

        //    return View(accountRegisterViewModel);
        //}

        //public ActionResult RegisterSuccess()
        //{
        //    var registerSuccessViewModel = _emptyViewModelFactory.Make();

        //    _viewModelBaseBuilder.Build(registerSuccessViewModel);

        //    return View(registerSuccessViewModel);
        //}

        #endregion

    }
}
