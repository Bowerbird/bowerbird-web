/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Team Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Members;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Members
{
    [TestFixture]
    public class TeamPostControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private Mock<IUserContext> _mockUserContext;
        private Mock<ICommandProcessor> _mockCommandProcessor;
        private TeamPostController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _mockUserContext = new Mock<IUserContext>();
            _mockCommandProcessor = new Mock<ICommandProcessor>();

            _controller = new TeamPostController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _documentStore.OpenSession()
                );
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests


        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_List_Returns_TeamList_In_Json_Format()
        {
            var user = FakeObjects.TestUserWithId();
            var team = FakeObjects.TestTeamWithId();
            var teamPosts = new List<TeamPost>();

            const int page = 1;
            const int pageSize = 10;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(team);
                session.SaveChanges();

                for (var i = 0; i < 15; i++)
                {
                    var teamPost = FakeObjects.TestTeamPostWithId(i.ToString());
                    teamPosts.Add(teamPost);
                    session.Store(teamPost);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new TeamPostListInput() { Page = page, PageSize = pageSize, TeamId = team.Id, UserId = user.Id });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsInstanceOf<TeamPostList>(jsonResult.Data);
            var jsonData = jsonResult.Data as TeamPostList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.Posts.PagedListItems.Count());
            Assert.AreEqual(teamPosts.Count, jsonData.Posts.TotalResultCount);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Create_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasTeamPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new TeamPostCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Create_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasTeamPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Create(new TeamPostCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Create_Having_Invalid_Permission_Returns_HttpUnauthorised()
        {
            _mockUserContext.Setup(x => x.HasTeamPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new TeamPostCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Update_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<TeamPost>(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new TeamPostUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Update_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<TeamPost>(It.IsAny<string>())).Returns(true);

            var result = _controller.Update(new TeamPostUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Update_Having_Invalid_Permission_Returns_HttpUnauthorised()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<TeamPost>(It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new TeamPostUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Delete_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<TeamPost>(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(new IdInput() { Id = FakeValues.KeyString });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Delete_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<TeamPost>(It.IsAny<string>())).Returns(true);

            var result = _controller.Delete(new IdInput() { Id = FakeValues.KeyString });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPost_Delete_Having_Invalid_Permission_Returns_HttpUnauthorised()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<TeamPost>(It.IsAny<string>())).Returns(false);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }


        #endregion 
    }
}