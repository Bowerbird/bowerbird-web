using System.Web.Mvc;
using Bowerbird.Core;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Tasks;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers;
using Bowerbird.Web.ViewModels;
using NUnit.Framework;
using Moq;

namespace Bowerbird.Web.Test.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {

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

        #region Constructor tests

        [Test]
        public void AccountController_Constructor_With_Null_CommandProcessor_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new AccountController(
                    null,
                    _mockViewModelRepository.Object,
                    _mockUserTasks.Object,
                    _mockUserContext.Object
                    )));
        }

        [Test]
        public void AccountController_Constructor_With_Null_ViewModelRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new AccountController(
                    _mockCommandProcessor.Object,
                    null,
                    _mockUserTasks.Object,
                    _mockUserContext.Object
                    )));
        }

        [Test]
        public void AccountController_Constructor_With_Null_UserTasks_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new AccountController(
                    _mockCommandProcessor.Object,
                    _mockViewModelRepository.Object,
                    null,
                    _mockUserContext.Object
                    )));
        }

        [Test]
        public void AccountController_Constructor_With_Null_UserContext_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new AccountController(
                    _mockCommandProcessor.Object,
                    _mockViewModelRepository.Object,
                    _mockUserTasks.Object,
                    null
                    )));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        public void AccountController_Login_Having_Unauthenticated_User_And_No_Cookie_Returns_Login_With_Empty_AccounLogin_ViewModel()
        {
            var mockAccountLogin = new AccountLogin() { Username = string.Empty };

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsFalse);
            _mockUserContext.Setup(x => x.HasUsernameCookieValue()).Returns(FakeValues.IsFalse);
            _mockViewModelRepository.Setup(x => x.Load<AccountLoginInput, AccountLogin>(It.IsAny<AccountLoginInput>())).Returns(mockAccountLogin);

            _controller.Login();

            var viewModel = _controller.ViewData.Model;

            Assert.IsInstanceOf<AccountLogin>(viewModel);
            Assert.IsTrue(((AccountLogin)viewModel).Username.Equals(string.Empty));
        }

        [Test]
        public void AccountController_Login_Having_Unauthenticated_User_And_Cookie_Returns_Login_With_AccounLogin_ViewModel_Having_Username()
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

        [Test]
        public void AccountController_Login_Having_Authenticated_User_Redirects_User_To_Home_Index()
        {
            var mockHomeIndex = new HomeIndex() {StreamItems = new PagedList<StreamItem>()};

            _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsTrue);
            _mockViewModelRepository.Setup(x => x.Load<HomeIndexInput, HomeIndex>(It.IsAny<HomeIndexInput>())).Returns(mockHomeIndex);

            var response = _controller.Login() as RedirectToRouteResult;

            Assert.IsTrue(response.RouteValues["controller"].Equals("home"));
            Assert.IsTrue(response.RouteValues["action"].Equals("index"));
        }

        #endregion

        #region Helpers

        #endregion

    }
}