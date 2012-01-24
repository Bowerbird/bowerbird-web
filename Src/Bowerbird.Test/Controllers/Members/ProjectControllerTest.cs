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
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
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
    public class ProjectControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private ProjectController _controller;
        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _controller = new ProjectController(
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
        public void Project_List_Returns_ProjectList_In_Json_Format()
        {
            var result = _controller.List(new ProjectListInput(){Page = 1, PageSize = 10, TeamId = FakeValues.KeyString});

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);
            
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            var jsonData = jsonResult.Data;

            Assert.IsInstanceOf<ProjectList>(jsonData);
            // more assertions for expected data...
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Index_NonAjaxCall_Returns_ProjectIndex_ViewModel_With_Project_Having_Observations_And_Team()
        {
            var team = FakeObjects.TestTeam();
            var project = FakeObjects.TestProjectWithId();
            project.Team = team;
            var observation1 = FakeObjects.TestObservationWithId();
            var observation2 = FakeObjects.TestObservationWithId(FakeValues.KeyString.AppendWith(FakeValues.KeyString));
            var projectobservation1 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation1);
            var projectobservation2 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation2);

            using (var session = _documentStore.OpenSession())
            {
                session.Store(observation1);
                session.Store(observation2);
                session.Store(project);
                session.Store(projectobservation1);
                session.Store(projectobservation2);

                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Index(new IdInput() { Id = project.Id });

            Assert.IsInstanceOf<ProjectIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as ProjectIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Project, project);
            Assert.IsNotNull(viewModel.Project.Team);
            Assert.AreEqual(viewModel.Project.Team.Name, team.Name);
            Assert.IsTrue(viewModel.Observations.Contains(observation1));
            Assert.IsTrue(viewModel.Observations.Contains(observation2));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Index_AjaxCall_Returns_ProjectIndex_Json_With_Project_Having_Observations_And_Team()
        {
            var team = FakeObjects.TestTeam();
            var project = FakeObjects.TestProjectWithId();
            project.Team = team;
            var observation1 = FakeObjects.TestObservationWithId();
            var observation2 = FakeObjects.TestObservationWithId(FakeValues.KeyString.AppendWith(FakeValues.KeyString));
            var projectobservation1 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation1);
            var projectobservation2 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation2);

            using (var session = _documentStore.OpenSession())
            {
                session.Store(observation1);
                session.Store(observation2);
                session.Store(project);
                session.Store(projectobservation1);
                session.Store(projectobservation2);

                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Index(new IdInput() { Id = project.Id });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ProjectIndex>(jsonResult.Data);

            var jsonData = jsonResult.Data as ProjectIndex;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(jsonData.Project, project);
            Assert.IsNotNull(jsonData.Project.Team);
            Assert.AreEqual(jsonData.Project.Team.Name, team.Name);
            Assert.IsTrue(jsonData.Observations.Contains(observation1));
            Assert.IsTrue(jsonData.Observations.Contains(observation2));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Create_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new ProjectCreateInput()
                                                {
                                                    Description = FakeValues.Description, 
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Create_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Create(new ProjectCreateInput()
                                                {
                                                    Description = FakeValues.Description, 
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Update_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Project>(It.IsAny<string>())).Returns(true);
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new ProjectUpdateInput()
                                                {
                                                    ProjectId = FakeValues.KeyString, 
                                                    Description = FakeValues.Description, 
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Update_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Project>(It.IsAny<string>())).Returns(true);
            var result = _controller.Update(new ProjectUpdateInput()
                                                {
                                                    ProjectId = FakeValues.KeyString, 
                                                    Description = FakeValues.Description, 
                                                    Name = FakeValues.Name
                                                });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Delete_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Project>(It.IsAny<string>())).Returns(true);
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Delete_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Project>(It.IsAny<string>())).Returns(true);
            var result = _controller.Delete(FakeViewModels.MakeIdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        #endregion 
    }
}