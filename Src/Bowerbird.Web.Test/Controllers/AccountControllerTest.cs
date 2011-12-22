/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

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

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_Constructor_With_Null_CommandProcessor_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(null,_mockViewModelRepository.Object,_mockUserTasks.Object,_mockUserContext.Object)));
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_Constructor_With_Null_ViewModelRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(_mockCommandProcessor.Object,null,_mockUserTasks.Object,_mockUserContext.Object)));
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_Constructor_With_Null_UserTasks_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(_mockCommandProcessor.Object,_mockViewModelRepository.Object,null,_mockUserContext.Object)));
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_Constructor_With_Null_UserContext_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountController(_mockCommandProcessor.Object,_mockViewModelRepository.Object,_mockUserTasks.Object,null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_HttpGet_Login_Having_Unauthenticated_User_And_No_Cookie_Returns_Login_With_Empty_AccounLogin_ViewModel()
        {
            var accountLogin = new AccountLogin() { Username = string.Empty };

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsFalse);
            _mockUserContext.Setup(x => x.HasUsernameCookieValue()).Returns(FakeValues.IsFalse);
            _mockViewModelRepository.Setup(x => x.Load<AccountLoginInput, AccountLogin>(It.IsAny<AccountLoginInput>())).Returns(accountLogin);

            _controller.Login();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<AccountLogin>(viewModel);
            Assert.IsTrue(((AccountLogin)viewModel).Username.Equals(string.Empty));
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_HttpGet_Login_Having_Unauthenticated_User_And_Cookie_Returns_Login_With_AccounLogin_ViewModel_Having_Username()
        {
            var mockAccountLogin = new AccountLogin() { Username = FakeValues.UserName };

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsFalse);
            _mockUserContext.Setup(x => x.HasUsernameCookieValue()).Returns(FakeValues.IsTrue);
            _mockUserContext.Setup(x => x.GetUsernameCookieValue()).Returns(FakeValues.UserName);
            _mockViewModelRepository.Setup(x => x.Load<AccountLoginInput, AccountLogin>(It.IsAny<AccountLoginInput>())).Returns(mockAccountLogin);

            _controller.Login();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<AccountLogin>(viewModel);
            Assert.IsTrue(((AccountLogin)viewModel).Username.Equals(FakeValues.UserName));
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_HttpGet_Login_Having_Authenticated_User_Redirects_User_To_Home_Index()
        {
            var mockHomeIndex = new HomeIndex() {StreamItems = new PagedList<StreamItem>()};

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsTrue);
            _mockViewModelRepository.Setup(x => x.Load<HomeIndexInput, HomeIndex>(It.IsAny<HomeIndexInput>())).Returns(mockHomeIndex);

            var response = _controller.Login() as RedirectToRouteResult;

            Assert.IsTrue(response.RouteValues["controller"].Equals("home"));
            Assert.IsTrue(response.RouteValues["action"].Equals("index"));
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_HttpPost_Login_Passing_Null_AccountLoginInput_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _controller.Login(null)));
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_HttpPost_Login_Passing_Invalid_Credentials_Returns_Login_View()
        {
            var accountLogin = new AccountLogin() { Username = string.Empty };
            var accountLoginInput = new AccountLoginInput(){ Username = FakeValues.UserName, Password = FakeValues.Password };

            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsFalse);
            _mockViewModelRepository.Setup(x => x.Load<AccountLoginInput, AccountLogin>(accountLoginInput)).Returns(accountLogin);

            _controller.Login(accountLoginInput);

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<AccountLogin>(viewModel);
        }

        [Test, Category(TestCategories.Integration)]
        public void AccountController_HttpPost_Login_Passing_Invalid_Credentials_Loads_LoginViewModel()
        {
            var accountLogin = new AccountLogin() { Username = string.Empty };
            var accountLoginInput = new AccountLoginInput() { Username = FakeValues.UserName, Password = FakeValues.Password };

            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsFalse);
            _mockViewModelRepository.Setup(x => x.Load<AccountLoginInput, AccountLogin>(accountLoginInput)).Returns(accountLogin);

            _controller.Login(accountLoginInput);

            _mockViewModelRepository.Verify(x => x.Load<AccountLoginInput, AccountLogin>(accountLoginInput), Times.Once());
        }

        [Test, Category(TestCategories.Unit)] 
        public void AccountController_HttpPost_Login_Passing_Valid_Credentials_Processes_LastLogin_SignsUserIn_And_Redirects_To_Url()
        {
            var returnUrl = "stuff";
            var accountLoginInput = new AccountLoginInput() { Username = FakeValues.UserName, Password = FakeValues.Password, ReturnUrl = returnUrl };

            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsTrue);
            _mockCommandProcessor.Setup(x => x.Process<UserUpdateLastLoginCommand>(It.IsAny<UserUpdateLastLoginCommand>()));

            var result = _controller.Login(accountLoginInput);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.IsTrue(((RedirectToRouteResult)result).RouteValues.ContainsKey("action"));
            Assert.AreEqual("loggingin", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
            Assert.IsTrue(((RedirectToRouteResult) result).RouteValues.ContainsKey("returnUrl"));
            Assert.AreEqual(returnUrl, ((RedirectToRouteResult)result).RouteValues["returnUrl"].ToString());
        }

        [Test, Category(TestCategories.Integration)]
        public void AccountController_HttpPost_Login_Passing_Valid_Credentials_Processes_UserUpdateLastLoginCommand_And_Calls_SignUserIn()
        {
            _mockUserTasks.Setup(x => x.AreCredentialsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(FakeValues.IsTrue);
            
            _controller.Login(new AccountLoginInput() { Username = FakeValues.UserName, Password = FakeValues.Password });

            _mockUserContext.Verify(x => x.SignUserIn(It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
            _mockCommandProcessor.Verify(x => x.Process<UserUpdateLastLoginCommand>(It.IsAny<UserUpdateLastLoginCommand>()), Times.Once());
        }

        [Test, Category(TestCategories.Integration)]
        public void AccountController_LoggingIn_Calls_UserContext_HasUserNameCookieValue()
        {
            _mockUserContext.Setup(x => x.HasUsernameCookieValue()).Returns(false);

            var result = _controller.LoggingIn(string.Empty);

            _mockUserContext.Verify(x => x.HasUsernameCookieValue(), Times.Once());
        }

        [Test, Category(TestCategories.Unit)]
        public void AccountController_LoggingIn_Not_Having_Cookie_RedirectsTo_Login()
        {
            _mockUserContext.Setup(x => x.HasUsernameCookieValue()).Returns(false);

            var result =  _controller.LoggingIn(string.Empty);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("login", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
        }

        [Test, Category(TestCategories.Unit)]
        public void AccountController_LoggingIn_Having_Cookie_With_ReturnUrl_RedirectsTo_ReturnUrl()
        {
            _mockUserContext.Setup(x => x.HasUsernameCookieValue()).Returns(true);

            var result = _controller.LoggingIn(string.Empty);

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("index", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
            Assert.AreEqual("home", ((RedirectToRouteResult)result).RouteValues["controller"].ToString());
        }

        [Test, Category(TestCategories.Unit)]
        public void AccountController_LoggingIn_Having_Cookie_Without_ReturnUrl_RedirectsTo_Home_Index()
        {
            _mockUserContext.Setup(x => x.HasUsernameCookieValue()).Returns(true);

            var result = _controller.LoggingIn(FakeValues.Website);

            Assert.IsInstanceOf<RedirectResult>(result);
            Assert.AreEqual(FakeValues.Website, ((RedirectResult)result).Url);
        }

        [Test, Category(TestCategories.Unit)]
        public void AccountController_Logout_RedirectsTo_Logoutsuccess()
        {
            var result = _controller.Logout();

            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            Assert.AreEqual("logoutsuccess", ((RedirectToRouteResult)result).RouteValues["action"].ToString());
        }

        [Test, Category(TestCategories.Integration)]
        public void AccountController_Logout_Calls_UserContext_SignUserOut()
        {
            var result = _controller.Logout();

            _mockUserContext.Verify(x => x.SignUserOut(), Times.Once());
        }

        [Test, Category(TestCategories.Unit)]
        public void AccountController_LogoutSuccess_Returns_DefaultViewModel()
        {
            _mockViewModelRepository.Setup(x => x.Load<DefaultViewModel>()).Returns(new DefaultViewModel());

            var result = _controller.LogoutSuccess();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<DefaultViewModel>(viewModel);
        }

        [Test, Category(TestCategories.Integration)]
        public void AccountController_LogoutSuccess_Calls_ViewModelRepository_Load()
        {
            _mockViewModelRepository.Setup(x => x.Load<DefaultViewModel>()).Returns(new DefaultViewModel());

            var result = _controller.LogoutSuccess();

            _mockViewModelRepository.Verify(x => x.Load<DefaultViewModel>(), Times.Once());
        }

        #endregion
    }
}