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
using System.Linq;
using System.Collections.Generic;
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
        public void Project_List_Having_TeamId_Returns_ProjectList_In_Json_Format()
        {
            var team = FakeObjects.TestTeamWithId();
            const int page = 1;
            const int pageSize = 10;
            
            var projects = new List<Project>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(team);

                for(var i = 0; i<15; i++)
                {
                    var project = FakeObjects.TestProjectWithId(i.ToString());
                    project.Team = team;
                    projects.Add(project);
                    session.Store(project);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new ProjectListInput(){Page = page, PageSize = pageSize, TeamId = team.Id});

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);
            
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<ProjectList>(jsonResult.Data);
            var jsonData = jsonResult.Data as ProjectList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.Projects.PagedListItems.Count());
            Assert.AreEqual(projects.Count, jsonData.Projects.TotalResultCount);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_List_Having_No_TeamId_Returns_ProjectList_In_Json_Format()
        {
            var team = FakeObjects.TestTeamWithId();
            const int page = 1;
            const int pageSize = 10;

            var projects = new List<Project>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(team);

                for (var i = 0; i < 15; i++)
                {
                    var project = FakeObjects.TestProjectWithId(i.ToString());
                    projects.Add(project);
                    session.Store(project);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new ProjectListInput() { Page = page, PageSize = pageSize });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<ProjectList>(jsonResult.Data);
            var jsonData = jsonResult.Data as ProjectList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.Projects.PagedListItems.Count());
            Assert.AreEqual(projects.Count, jsonData.Projects.TotalResultCount);
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
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(true);

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
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(true);

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
        public void Project_Create_Having_Invalid_Permission_Returns_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new ProjectCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
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
        public void Project_Update_Having_Invalid_Permission_Returns_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Project>(It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new ProjectUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
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

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Delete_Having_Invalid_Permission_Returns_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Project>(It.IsAny<string>())).Returns(false);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        #endregion 
    }
}