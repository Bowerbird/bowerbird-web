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

using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Members;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Moq;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Members
{
    [TestFixture]
    public class ObservationControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private ObservationController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _controller = new ObservationController(
                _mockCommandProcessor.Object,
                _documentStore.OpenSession(),
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
        public void Observation_Index_NonAjaxCall_Returns_ObservationIndex_ViewModel()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(observation);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Index(new IdInput() { Id = observation.Id });

            Assert.IsInstanceOf<ObservationIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as ObservationIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Observation, observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Index_AjaxCall_Returns_ObservationIndex_Json()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(observation);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Index(new IdInput() { Id = observation.Id });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ObservationIndex>(jsonResult.Data);

            var jsonData = jsonResult.Data as ObservationIndex;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(observation, jsonData.Observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_List_Having_UserId_Returns_ObservationList_Having_Users_Observations_In_Json_Format()
        {
            var user = FakeObjects.TestUserWithId();
            const int page = 1;
            const int pageSize = 10;

            var observations = new List<Observation>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);

                for (var i = 0; i < 15; i++)
                {
                    var observation = FakeObjects.TestObservationWithId(i.ToString());
                    observations.Add(observation);
                    session.Store(observation);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new ObservationListInput() { Page = page, PageSize = pageSize, UserId = user.Id});

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<ObservationList>(jsonResult.Data);
            var jsonData = jsonResult.Data as ObservationList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);
            Assert.AreEqual(user.Id, jsonData.UserId);
        }

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void Observation_List_Having_ProjectId_Returns_ObservationList_Having_Projects_Observations_In_Json_Format()
        {
            //var user = FakeObjects.TestUserWithId();
            //var project = FakeObjects.TestProjectWithId();
            //const int page = 1;
            //const int pageSize = 10;

            //var observations = new List<Observation>();

            //using (var session = _documentStore.OpenSession())
            //{
            //    session.Store(user);
            //    session.Store(project);

            //    for (var i = 0; i < 15; i++)
            //    {
            //        var observation = FakeObjects.TestObservationWithId(i.ToString());
            //        var projectObservation = new ProjectObservation(
            //            user,
            //            FakeValues.CreatedDateTime,
            //            project,
            //            observation);

            //        observations.Add(observation);
            //        session.Store(projectObservation);
            //        session.Store(observation);
            //    }

            //    session.SaveChanges();
            //}

            //var result = _controller.List(new ObservationListInput() { Page = page, PageSize = pageSize, UserId = user.Id, ProjectId = project.Id});

            //Assert.IsNotNull(result);
            //Assert.IsInstanceOf<JsonResult>(result);

            //var jsonResult = result as JsonResult;
            //Assert.IsNotNull(jsonResult);

            //Assert.IsNotNull(jsonResult.Data);
            //Assert.IsInstanceOf<ObservationList>(jsonResult.Data);
            //var jsonData = jsonResult.Data as ObservationList;

            //Assert.IsNotNull(jsonData);
            //Assert.AreEqual(page, jsonData.Page);
            //Assert.AreEqual(pageSize, jsonData.PageSize);
            //Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            //Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);
            //Assert.AreEqual(user.Id, jsonData.UserId);
            //Assert.AreEqual(project, jsonData.Project);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Create_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new ObservationCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Create_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(true);

            var result = _controller.Create(new ObservationCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Create_Having_Invalid_Permissions_Returns_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new ObservationCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Update_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Observation>(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new ObservationUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Update_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Observation>(It.IsAny<string>())).Returns(true);

            var result = _controller.Update(new ObservationUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Update_Having_Invalid_Permissions_Returns_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Observation>(It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new ObservationUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Delete_Passing_Invalid_Input_Returns_Json_Error()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Observation>(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Delete_Passing_Valid_Input_Returns_Json_Success()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Observation>(It.IsAny<string>())).Returns(true);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Delete_Having_Invalid_Permissions_Returns_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Observation>(It.IsAny<string>())).Returns(false);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        #endregion
    }
}