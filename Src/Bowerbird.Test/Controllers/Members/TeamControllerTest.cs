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


using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Extensions;
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
    public class TeamControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private TeamController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _documentStore = DocumentStoreHelper.TestDocumentStore();

            _controller = new TeamController(
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
        public void Team_List_Returns_Json_Success()
        {
            var result = _controller.List(null, null, null);

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Index_NonAjaxCall_Returns_TeamIndex_Json_Having_Projects()
        {
            var team = FakeObjects.TestTeamWithId();
            var project = FakeObjects.TestProjectWithId();
            project.Team = team;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(project);
                session.Store(team);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Index(new IdInput() { Id = team.Id });

            Assert.IsInstanceOf<TeamIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as TeamIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Team, team);
            Assert.IsNotNull(viewModel.Projects);
            Assert.IsTrue(viewModel.Projects.Contains(project));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Index_AjaxCall_Returns_TeamIndex_Json_Having_Projects()
        {
            var team = FakeObjects.TestTeamWithId();
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();
            project.Team = team;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(team);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Index(new IdInput() { Id = team.Id });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<TeamIndex>(jsonResult.Data);

            var jsonData = jsonResult.Data as TeamIndex;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(jsonData.Team, team);
            Assert.IsNotNull(jsonData.Projects);
            Assert.IsTrue(jsonData.Projects.Contains(project));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Create_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new TeamCreateInput()
            {
                Description = FakeValues.Description,
                Name = FakeValues.Name
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Create_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Create(new TeamCreateInput()
            {
                Description = FakeValues.Description,
                Name = FakeValues.Name
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Update_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new TeamUpdateInput()
            {
                Id = FakeValues.KeyString,
                Description = FakeValues.Description,
                Name = FakeValues.Name
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Update_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Update(new TeamUpdateInput()
            {
                Id = FakeValues.KeyString,
                Description = FakeValues.Description,
                Name = FakeValues.Name
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Delete_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Delete_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }


        #endregion 
    }
}
