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
using Bowerbird.Core.Config;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Controllers;
using Bowerbird.Web.ViewModels.Account;
using Bowerbird.Web.ViewModels.User;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Web.Controllers
{
    [TestFixture]
    public class UsersControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private UsersController _controller;
        private Mock<IUserViewModelBuilder> _mockViewModelBuilder;
        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _mockViewModelBuilder = new Mock<IUserViewModelBuilder>();

            _controller = new UsersController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _mockViewModelBuilder.Object
                );
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;
            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UsersController_Update_Get_Returns_View_With_UserUpdate_Model_Populated_With_Users_Data()
        {
            var user = FakeObjects.TestUserWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);

                session.SaveChanges();
            }

            _mockUserContext.Setup(x => x.GetAuthenticatedUserId()).Returns(user.Id);

            var result = _controller.Update() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model;
            Assert.IsInstanceOf<UserUpdate>(model);

            var modelData = result.Model as UserUpdate;
            Assert.IsNotNull(modelData);

            Assert.AreEqual(user.Description, modelData.Description);
            Assert.AreEqual(user.FirstName, modelData.FirstName);
            Assert.AreEqual(user.LastName, modelData.LastName);
            Assert.AreEqual(user.Email, modelData.Email);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UsersController_Update_Post_Passing_Valid_UserUpdateInput_Redirects_To_Home_Index()
        {
            var result = _controller.Update(new UserUpdateInput() { FirstName = FakeValues.Name, LastName = FakeValues.Name, Email = FakeValues.Email });

            var routeResult = result as RedirectToRouteResult;

            Assert.IsNotNull(routeResult);
            Assert.AreEqual(routeResult.RouteValues["controller"], "home");
            Assert.AreEqual(routeResult.RouteValues["action"], "index");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UsersController_Update_Passing_InValid_UserUpdateInput_Returns_Populated_UserUpdate_Model()
        {
            var user = FakeObjects.TestUserWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.SaveChanges();
            }

            var userUpdateInput = new UserUpdateInput()
            {
                Description = FakeValues.Description,
                Email = FakeValues.Email,
                FirstName = FakeValues.FirstName,
                LastName = FakeValues.LastName
            };

            _mockUserContext.Setup(x => x.GetAuthenticatedUserId()).Returns(user.Id);
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(userUpdateInput) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model;
            Assert.IsInstanceOf<UserUpdate>(model);

            var modelData = result.Model as UserUpdate;
            Assert.IsNotNull(modelData);

            Assert.AreEqual(userUpdateInput.Description, modelData.Description);
            Assert.AreEqual(userUpdateInput.FirstName, modelData.FirstName);
            Assert.AreEqual(userUpdateInput.LastName, modelData.LastName);
            Assert.AreEqual(userUpdateInput.Email, modelData.Email);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UsersController_ChangePassword_Passing_Valid_AccountChangePasswordInput_Redirects_To_Home_Index()
        {
            var result = _controller.ChangePassword(new AccountChangePasswordInput() { Password = FakeValues.Password });

            var routeResult = result as RedirectToRouteResult;

            Assert.IsNotNull(routeResult);
            Assert.AreEqual(routeResult.RouteValues["controller"], "home");
            Assert.AreEqual(routeResult.RouteValues["action"], "index");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UsersController_ChangePassword_Passing_InValid_AccountChangePasswordInput_Returns_View()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.ChangePassword(new AccountChangePasswordInput()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ChangePassword", result.ViewName);
        }

        #endregion
    }
}