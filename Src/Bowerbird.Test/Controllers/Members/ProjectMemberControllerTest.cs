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
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Members;
using Bowerbird.Web.ViewModels.Members;
using NUnit.Framework;
using Moq;
using Bowerbird.Test.Utils;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Members
{
    [TestFixture]
    public class ProjectMemberControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private ProjectMemberController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _documentStore = DocumentStoreHelper.TestDocumentStore();

            _controller = new ProjectMemberController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _documentStore.OpenSession());
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_List_Returns_Json_Success()
        {
            var result = _controller.List(null, null, null);

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Create_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new ProjectMemberCreateInput()
                                                {
                                                    UserId = FakeValues.UserId, 
                                                    ProjectId = FakeValues.KeyString, 
                                                    Roles = FakeValues.StringList
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Create_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Create(new ProjectMemberCreateInput()
                                                {
                                                    UserId = FakeValues.UserId, 
                                                    ProjectId = FakeValues.KeyString, 
                                                    Roles = FakeValues.StringList
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Delete_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(new ProjectMemberDeleteInput()
                                                {
                                                    ProjectId = FakeValues.KeyString, 
                                                    ProjectMemberId = FakeValues.KeyString
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Delete_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Delete(new ProjectMemberDeleteInput()
                                                {
                                                    ProjectId = FakeValues.KeyString, 
                                                    ProjectMemberId = FakeValues.KeyString
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        #endregion
    }
}