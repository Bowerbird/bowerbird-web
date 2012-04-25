/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers;
using Bowerbird.Web.ViewModels;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Members
{
    [TestFixture]
    public class OrganisationControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private Mock<IUserContext> _mockUserContext;
        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IConfigService> _mockConfigService;
        private Mock<IMediaFilePathService> _mockMediaFilePathService;
        private OrganisationsController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.InMemoryDocumentStore();
            _mockUserContext = new Mock<IUserContext>();
            _mockConfigService = new Mock<IConfigService>();
            _mockMediaFilePathService = new Mock<IMediaFilePathService>();
            _mockCommandProcessor = new Mock<ICommandProcessor>();

            _controller = new OrganisationsController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _documentStore.OpenSession(),
                _mockMediaFilePathService.Object,
                _mockConfigService.Object
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

        #region Methods

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Index_ViewModel()
        {
            var organisation = FakeObjects.TestOrganisationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(organisation);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Index(new IdInput() { Id = organisation.Id });

            Assert.IsInstanceOf<OrganisationIndex>(_controller.ViewData.Model);

            var jsonData = _controller.ViewData.Model as OrganisationIndex;

            Assert.IsNotNull(jsonData);

            Assert.AreEqual(jsonData.Name, organisation.Name);
            Assert.AreEqual(jsonData.Description, organisation.Description);
            Assert.AreEqual(jsonData.Website, organisation.Website);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Index_Json()
        {
            var organisation = FakeObjects.TestOrganisationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(organisation);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Index(new IdInput() { Id = organisation.Id });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsInstanceOf<OrganisationIndex>(jsonResult.Data);
            var jsonData = jsonResult.Data as OrganisationIndex;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(jsonData.Name, organisation.Name);
            Assert.AreEqual(jsonData.Description, organisation.Description);
            Assert.AreEqual(jsonData.Website, organisation.Website);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_List_Json()
        {
            var user = FakeObjects.TestUserWithId();
            var organisations = new List<Organisation>();

            const int page = 1;
            const int pageSize = 10;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);

                for (var i = 0; i < 15; i++)
                {
                    var organisation = FakeObjects.TestOrganisationWithId(i.ToString());
                    organisations.Add(organisation);
                    session.Store(organisation);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new OrganisationListInput() { Page = page, PageSize = pageSize });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<OrganisationList>(jsonResult.Data);
            var jsonData = jsonResult.Data as OrganisationList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.Organisations.PagedListItems.Count());
            Assert.AreEqual(organisations.Count, jsonData.Organisations.TotalResultCount);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Create_Invalid_Input()
        {
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new OrganisationCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Create()
        {
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(true);

            var result = _controller.Create(new OrganisationCreateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Create_Invalid_Permission()
        {
            _mockUserContext.Setup(x => x.HasGlobalPermission(It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new OrganisationCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Update_Invalid_Input()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Organisation>(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Update(new OrganisationUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Update()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Organisation>(It.IsAny<string>())).Returns(true);

            var result = _controller.Update(new OrganisationUpdateInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Update_Invalid_Permission()
        {
            _mockUserContext.Setup(x => x.HasPermissionToUpdate<Organisation>(It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new OrganisationUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Delete_Invalid_Input()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Organisation>(It.IsAny<string>())).Returns(true);

            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Failure".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Delete()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Organisation>(It.IsAny<string>())).Returns(true);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Delete_Invalid_Permission()
        {
            _mockUserContext.Setup(x => x.HasPermissionToDelete<Organisation>(It.IsAny<string>())).Returns(false);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        #endregion
    }
}