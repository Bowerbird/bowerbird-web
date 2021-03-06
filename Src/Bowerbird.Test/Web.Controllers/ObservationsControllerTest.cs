﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com

 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au

 Funded by:
 * Atlas of Living Australia

*/

using System.Web.Mvc;
using System.Collections.Generic;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Controllers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.ViewModels;
using NUnit.Framework;
using Moq;
using Raven.Client;

namespace Bowerbird.Test.Web.Controllers
{
    [TestFixture]
    public class ObservationControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private Mock<IObservationsViewModelBuilder> _mockObservationsViewModelBuilder;
        private IDocumentStore _documentStore;
        private ObservationsController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockObservationsViewModelBuilder = new Mock<IObservationsViewModelBuilder>();
            _mockUserContext = new Mock<IUserContext>();

            _controller = new ObservationsController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _mockObservationsViewModelBuilder.Object
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

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_GetOne_As_ViewModel()
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

            _controller.GetOne(new IdInput() { Id = observation.Id });

            //Assert.IsInstanceOf<ObservationIndex>(_controller.ViewData.Model);

            //var viewModel = _controller.ViewData.Model as ObservationIndex;

            //Assert.IsNotNull(viewModel);
            //Assert.AreEqual(viewModel.Observation, observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_GetOne_As_Json()
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

            var result = _controller.GetOne(new IdInput() { Id = observation.Id });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            //Assert.IsInstanceOf<ObservationIndex>(jsonResult.Data);

            //var jsonData = jsonResult.Data as ObservationIndex;
            //Assert.IsNotNull(jsonData);

            //Assert.AreEqual(observation, jsonData.Observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_GetMany_By_User_As_ViewModel()
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

            var result = _controller.GetMany(new PagingInput() { Page = page, PageSize = pageSize });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<object>(jsonResult.Data);
            var jsonData = jsonResult.Data as object;

            Assert.IsNotNull(jsonData);
            //Assert.AreEqual(page, jsonData.Page);
            //Assert.AreEqual(pageSize, jsonData.PageSize);
            //Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            //Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);
            //Assert.AreEqual(user, jsonData.CreatedByUser);
        }

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void Observation_GetMany_By_Project_As_ViewModel()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();

            const int page = 1;
            const int pageSize = 10;

            var observations = new List<Observation>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);

                for (var i = 0; i < 15; i++)
                {
                    var observation = FakeObjects.TestObservationWithId(i.ToString());
                    //var contribution = new GroupContribution(
                    //    project,
                    //    observation,
                    //    user,
                    //    FakeValues.CreatedDateTime
                    //    );

                    observations.Add(observation);
                    session.Store(observation);
                    //session.Store(contribution);
                }

                session.SaveChanges();
            }

            var result = _controller.GetMany(new PagingInput() { Page = page, PageSize = pageSize, Id = project.Id });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<object>(jsonResult.Data);
            var jsonData = jsonResult.Data as object;

            Assert.IsNotNull(jsonData);
            //Assert.AreEqual(page, jsonData.Page);
            //Assert.AreEqual(pageSize, jsonData.PageSize);
            //Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            //Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);
            //Assert.AreEqual(project, jsonData.Project);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_GetMany_By_User_As_Json()
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

            var result = _controller.GetMany(new PagingInput() { Page = page, PageSize = pageSize });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<object>(jsonResult.Data);
            var jsonData = jsonResult.Data as object;

            Assert.IsNotNull(jsonData);
            //Assert.AreEqual(page, jsonData.Page);
            //Assert.AreEqual(pageSize, jsonData.PageSize);
            //Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            //Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);
           // Assert.AreEqual(user, jsonData.CreatedByUser);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_GetMany_By_Project_In_Json_Format()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();

            const int page = 1;
            const int pageSize = 10;

            var observations = new List<Observation>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);

                for (var i = 0; i < 15; i++)
                {
                    var observation = FakeObjects.TestObservationWithId(i.ToString());
                    //var contribution = new GroupContribution(
                    //    project,
                    //    observation,
                    //    user,
                    //    FakeValues.CreatedDateTime
                    //    );

                    observations.Add(observation);
                    session.Store(observation);
                    //session.Store(contribution);
                }

                session.SaveChanges();
            }

            var result = _controller.GetMany(new PagingInput() { Page = page, PageSize = pageSize, Id = project.Id });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<object>(jsonResult.Data);
            var jsonData = jsonResult.Data as object;

            Assert.IsNotNull(jsonData);
            //Assert.AreEqual(page, jsonData.Page);
            //Assert.AreEqual(pageSize, jsonData.PageSize);
            //Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            //Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);
            //Assert.AreEqual(project, jsonData.Project);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Create_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new ObservationCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Create()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Create(new ObservationCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Create_With_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new ObservationCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Update_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new ObservationUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Update()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Update(new ObservationUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Update_With_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new ObservationUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Delete_With_Error()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Delete()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Delete_With_HttpUnauthorized()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        #endregion
    }
}