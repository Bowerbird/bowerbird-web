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
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Members;
using Bowerbird.Web.ViewModels.Members;
using Moq;
using NUnit.Framework;

namespace Bowerbird.Test.Controllers.Members
{
    [TestFixture]
    public class AccountControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private AccountController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();

            _controller = new AccountController(
                _mockCommandProcessor.Object,
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

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void AccountController_ChangePassword_Passing_Valid_AccountChangePasswordInput_Redirects_To_Home_Index()
        {
            var result = _controller.ChangePassword(new AccountChangePasswordInput() { Password = FakeValues.Password });

            var routeResult = result as RedirectToRouteResult;

            Assert.IsNotNull(routeResult);
            Assert.AreEqual(routeResult.RouteValues["controller"], "home");
            Assert.AreEqual(routeResult.RouteValues["action"], "index");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void AccountController_ChangePassword_Passing_InValid_AccountChangePasswordInput_Returns_View()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.ChangePassword(new AccountChangePasswordInput()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ChangePassword", result.ViewName);
        }

        #endregion
    }
}