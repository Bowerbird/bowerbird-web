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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Services;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers.Public;
using Bowerbird.Web.ViewModels.Shared;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Public
{
    [TestFixture]
    public class ProjectControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private Mock<IMediaFilePathService> _mockMediaFilePathService;
        private Mock<IConfigService> _mockConfigService;
        private ProjectController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.InMemoryDocumentStore();
            _mockMediaFilePathService = new Mock<IMediaFilePathService>();
            _mockConfigService = new Mock<IConfigService>();
            _controller = new ProjectController(
                _documentStore.OpenSession(),
                _mockMediaFilePathService.Object,
                _mockConfigService.Object);
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

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_List_In_Json_Format()
        {
            const int page = 1;
            const int pageSize = 10;

            var projects = new List<Project>();

            using (var session = _documentStore.OpenSession())
            {
                for (var i = 0; i < 15; i++)
                {
                    var project = FakeObjects.TestProjectWithId(i.ToString());
                    projects.Add(project);
                    session.Store(project);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new ProjectListInput() { Page = page, PageSize = pageSize});

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
        public void Project_Index_As_ViewModel()
        {
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(project);
                session.Store(user);

                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Index(new IdInput() { Id = project.Id });

            Assert.IsInstanceOf<ProjectIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as ProjectIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Project, project);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Index_As_Json()
        {
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(project);
                session.Store(user);

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
        }

        #endregion
    }
}