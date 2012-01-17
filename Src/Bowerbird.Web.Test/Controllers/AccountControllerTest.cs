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

namespace Bowerbird.Web.Test.Controllers
{
    #region Namespaces

    using System.Web.Mvc;

    using NUnit.Framework;
    using Moq;
    
    using Bowerbird.Core;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Tasks;
    using Bowerbird.Test.Utils;
    using Bowerbird.Web.Config;
    using Bowerbird.Web.Controllers;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Core.CommandHandlers;
    
    #endregion

    [TestFixture] 
    public class AccountControllerTest
    {
        #region Test Infrastructure
        
        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IViewModelRepository> _mockViewModelRepository;
        private Mock<IUserTasks> _mockUserTasks;
        private Mock<IUserContext> _mockUserContext;
        private AccountController _controller;

        [SetUp] 
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockViewModelRepository = new Mock<IViewModelRepository>();
            _mockUserTasks = new Mock<IUserTasks>();
            _mockUserContext = new Mock<IUserContext>();

            _controller = new AccountController(
                _mockCommandProcessor.Object,
                _mockViewModelRepository.Object,
                _mockUserTasks.Object,
                _mockUserContext.Object
                );
        }

        [TearDown] 
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_Constructor_With_Null_CommandProcessor_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(null,_mockViewModelRepository.Object,_mockUserTasks.Object,_mockUserContext.Object)));
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_Constructor_With_Null_ViewModelRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(_mockCommandProcessor.Object,null,_mockUserTasks.Object,_mockUserContext.Object)));
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_Constructor_With_Null_UserTasks_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(_mockCommandProcessor.Object,_mockViewModelRepository.Object,null,_mockUserContext.Object)));
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_Constructor_With_Null_UserContext_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(_mockCommandProcessor.Object,_mockViewModelRepository.Object,_mockUserTasks.Object,null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_HttpGet_Login_Having_Unauthenticated_User_And_No_Cookie_Returns_Login_With_Empty_AccounLogin_ViewModel()
        {
            var accountLogin = new AccountLogin() { Email = string.Empty };

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsFalse);
            _mockUserContext.Setup(x => x.HasEmailCookieValue()).Returns(FakeValues.IsFalse);
            _mockViewModelRepository.Setup(x => x.Load<AccountLogin>()).Returns(accountLogin);

            _controller.Login();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<AccountLogin>(viewModel);
            Assert.IsTrue(((AccountLogin)viewModel).Email.Equals(string.Empty));
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_HttpGet_Login_Having_Unauthenticated_User_And_Cookie_Returns_Login_With_AccounLogin_ViewModel_Having_Email()
        {
            var mockAccountLogin = new AccountLogin() { Email = FakeValues.Email };

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsFalse);
            _mockUserContext.Setup(x => x.HasEmailCookieValue()).Returns(FakeValues.IsTrue);
            _mockUserContext.Setup(x => x.GetEmailCookieValue()).Returns(FakeValues.Email);
            _mockViewModelRepository.Setup(x => x.Load<AccountLogin>()).Returns(mockAccountLogin);

            _controller.Login();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<AccountLogin>(viewModel);
            Assert.IsTrue(((AccountLogin)viewModel).Email.Equals(FakeValues.Email));
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_HttpGet_Login_Having_Authenticated_User_Redirects_User_To_Home_Index()
        {
            var mockHomeIndex = new HomeIndex() {StreamItems = new PagedList<StreamItem>()};

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsTrue);
            _mockViewModelRepository.Setup(x => x.Load<HomeIndexInput, HomeIndex>(It.IsAny<HomeIndexInput>())).Returns(mockHomeIndex);

            var result = _controller.Login();

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.IsTrue(((RedirectToRouteResult)result).RouteValues["controller"].Equals("home"));
            Assert.IsTrue(((RedirectToRouteResult)result).RouteValues["action"].Equals("index"));
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_HttpPost_Login_Passing_Null_AccountLoginInput_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _controller.Login(null)));
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_HttpPost_Login_Passing_Invalid_Credentials_Returns_Login_View()
        {
            var accountLogin = new AccountLogin() { Email = string.Empty };
            var accountLoginInput = new AccountLoginInput(){ Email = FakeValues.Email, Password = FakeValues.Password };

            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsFalse);
            _mockViewModelRepository.Setup(x => x.Load<AccountLoginInput, AccountLogin>(accountLoginInput)).Returns(accountLogin);

            _controller.Login(accountLoginInput);

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<AccountLogin>(viewModel);
        }

        [Test, Category(TestCategory.Integration)]
        public void AccountController_HttpPost_Login_Passing_Invalid_Credentials_Loads_LoginViewModel()
        {
            var accountLogin = new AccountLogin() { Email = string.Empty };
            var accountLoginInput = new AccountLoginInput() { Email = FakeValues.Email, Password = FakeValues.Password };

            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsFalse);
            _mockViewModelRepository.Setup(x => x.Load<AccountLoginInput, AccountLogin>(accountLoginInput)).Returns(accountLogin);

            _controller.Login(accountLoginInput);

            _mockViewModelRepository.Verify(x => x.Load<AccountLoginInput, AccountLogin>(accountLoginInput), Times.Once());
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_HttpPost_Login_Passing_Valid_Credentials_Signs_User_In_And_Updates_Last_Logged_In_And_Redirects_To_Logging_In_Action()
        {
            var accountLoginInput = new AccountLoginInput() { Email = FakeValues.Email, Password = FakeValues.Password };

            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsTrue);
            _mockCommandProcessor.Setup(x => x.Process<UserUpdateLastLoginCommand>(It.IsAny<UserUpdateLastLoginCommand>()));

            var result = _controller.Login(accountLoginInput);

            _mockUserContext.Verify(x => x.SignUserIn(It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            _mockCommandProcessor.Verify(x => x.Process<UserUpdateLastLoginCommand>(It.IsAny<UserUpdateLastLoginCommand>()), Times.Once());

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.IsTrue(((RedirectToRouteResult)result).RouteValues.ContainsKey("action"));
            Assert.AreEqual("loggingin", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountController_HttpPost_Login_Passing_Valid_Credentials_And_ReturnUrl_Redirects_To_Url()
        {
            var returnUrl = "stuff";
            var accountLoginInput = new AccountLoginInput() { Email = FakeValues.Email, Password = FakeValues.Password, ReturnUrl = returnUrl };

            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsTrue);
            _mockCommandProcessor.Setup(x => x.Process<UserUpdateLastLoginCommand>(It.IsAny<UserUpdateLastLoginCommand>()));

            var result = _controller.Login(accountLoginInput);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.IsTrue(((RedirectToRouteResult)result).RouteValues.ContainsKey("action"));
            Assert.AreEqual("loggingin", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
            Assert.IsTrue(((RedirectToRouteResult) result).RouteValues.ContainsKey("returnUrl"));
            Assert.AreEqual(returnUrl, ((RedirectToRouteResult)result).RouteValues["returnUrl"].ToString());
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_LoggingIn_Not_Having_Cookie_RedirectsTo_Login()
        {
            _mockUserContext.Setup(x => x.HasEmailCookieValue()).Returns(false);

            var result =  _controller.LoggingIn(string.Empty);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("login", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_LoggingIn_Having_Cookie_With_ReturnUrl_RedirectsTo_ReturnUrl()
        {
            _mockUserContext.Setup(x => x.HasEmailCookieValue()).Returns(true);

            var result = _controller.LoggingIn(string.Empty);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("index", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
            Assert.AreEqual("home", ((RedirectToRouteResult)result).RouteValues["controller"].ToString());
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_LoggingIn_Having_Cookie_Without_ReturnUrl_RedirectsTo_Home_Index()
        {
            _mockUserContext.Setup(x => x.HasEmailCookieValue()).Returns(true);

            var result = _controller.LoggingIn(FakeValues.Website);

            Assert.IsInstanceOf<RedirectResult>(result);
            Assert.AreEqual(FakeValues.Website, ((RedirectResult)result).Url);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_Logout_RedirectsTo_Logoutsuccess()
        {
            var result = _controller.Logout();

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("logoutsuccess", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
        }

        [Test, Category(TestCategory.Integration)]
        public void AccountController_Logout_Calls_UserContext_SignUserOut()
        {
            _controller.Logout();

            _mockUserContext.Verify(x => x.SignUserOut(), Times.Once());
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_LogoutSuccess_Returns_DefaultViewModel()
        {
            _mockViewModelRepository.Setup(x => x.Load<DefaultViewModel>()).Returns(new DefaultViewModel());

            _controller.LogoutSuccess();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<DefaultViewModel>(viewModel);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_HttpGet_Register_Returns_AccountRegisterViewModel()
        {
            var accountRegister = new AccountRegister();

            _mockViewModelRepository.Setup(x => x.Load<AccountRegister>()).Returns(accountRegister);

            var result = _controller.Register();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<AccountRegister>(viewModel);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountController_HttpPost_Register_Passing_Valid_Register_Details_RedirectsTo_RegisterSuccess()
        {
            var result =_controller.Register(FakeViewModels.MakeAccountRegisterInput());

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("index", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
            Assert.AreEqual("home", ((RedirectToRouteResult)result).RouteValues["controller"].ToString());
        }

        [Test, Category(TestCategory.Integration)]
        public void AccountController_HttpPost_Register_Passing_Valid_Register_Details_Registers_User()
        {
            var accountRegisterInput = FakeViewModels.MakeAccountRegisterInput();

            Func<UserCreateCommand, bool> isUserCreateCommandValid =
                x => x.FirstName != accountRegisterInput.FirstName ||
                     x.LastName != accountRegisterInput.LastName ||
                     x.Email != accountRegisterInput.Email ||
                     x.Password != accountRegisterInput.Password;

            _mockCommandProcessor.Setup(x => x.Process(It.Is<UserCreateCommand>(y => !isUserCreateCommandValid(y))))
                .Verifiable("AccountRegisterInput was not mapped to UserCreateCommand correctly");

            _controller.Register(accountRegisterInput);

            _mockCommandProcessor.Verify();
        }

        [Test, Category(TestCategory.Integration)]
        public void AccountController_HttpPost_Register_Passing_Invalid_Register_Details_Returns_AccountRegisterViewModel()
        {
            var accountRegisterInput = FakeViewModels.MakeAccountRegisterInput();

            _mockViewModelRepository
                .Setup(x => x.Load<AccountRegisterInput, AccountRegister>(It.IsAny<AccountRegisterInput>()))
                .Returns(FakeViewModels.MakeAccountRegister());

            _controller.ModelState.AddModelError("something", "invalid model state");

            var result = _controller.Register(accountRegisterInput);

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<AccountRegister>(viewModel);
            _mockCommandProcessor.Verify(x => x.Process(It.IsAny<UserCreateCommand>()), Times.Never());
        }

        #endregion
    }
}