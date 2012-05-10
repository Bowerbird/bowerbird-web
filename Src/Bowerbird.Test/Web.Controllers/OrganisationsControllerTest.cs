/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Controllers;
using Bowerbird.Web.ViewModels;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Web.Controllers
{
    [TestFixture]
    public class OrganisationsControllerTest
    {
        #region Test Infrastructure

        private Mock<IUserContext> _mockUserContext;
        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IOrganisationsViewModelBuilder> _mockOrganisationsViewModelBuilder;
        private Mock<IStreamItemsViewModelBuilder> _mockStreamItemsViewModelBuilder;
        private Mock<ITeamsViewModelBuilder> _mockTeamsViewModelBuilder;
        private Mock<IPostsViewModelBuilder> _mockPostsViewModelBuilder;
        private Mock<IMemberViewModelBuilder> _mockMemberViewModelBuilder;
        private Mock<IReferenceSpeciesViewModelBuilder> _mockReferenceSpeciesViewModelBuilder;
        private IDocumentStore _documentStore;
        private OrganisationsController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();
            _mockUserContext = new Mock<IUserContext>();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockOrganisationsViewModelBuilder = new Mock<IOrganisationsViewModelBuilder>();
            _mockStreamItemsViewModelBuilder = new Mock<IStreamItemsViewModelBuilder>();
            _mockTeamsViewModelBuilder = new Mock<ITeamsViewModelBuilder>();
            _mockPostsViewModelBuilder = new Mock<IPostsViewModelBuilder>();
            _mockMemberViewModelBuilder = new Mock<IMemberViewModelBuilder>();
            _mockReferenceSpeciesViewModelBuilder = new Mock<IReferenceSpeciesViewModelBuilder>();

            _controller = new OrganisationsController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _mockOrganisationsViewModelBuilder.Object,
                _mockStreamItemsViewModelBuilder.Object,
                _mockTeamsViewModelBuilder.Object,
                _mockPostsViewModelBuilder.Object,
                _mockMemberViewModelBuilder.Object,
                _mockReferenceSpeciesViewModelBuilder.Object
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

        #region Methods

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Stream_ViewModel()
        {
            var organisation = FakeObjects.TestOrganisationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(organisation);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Stream(new PagingInput() { Id = organisation.Id });

            Assert.IsInstanceOf<object>(_controller.ViewData.Model);

            var jsonData = _controller.ViewData.Model as object;

            Assert.IsNotNull(jsonData);

            //Assert.AreEqual(jsonData.Name, organisation.Name);
            //Assert.AreEqual(jsonData.Description, organisation.Description);
            //Assert.AreEqual(jsonData.Website, organisation.Website);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Stream_Json()
        {
            var organisation = FakeObjects.TestOrganisationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(organisation);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Stream(new PagingInput() { Id = organisation.Id });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsInstanceOf<object>(jsonResult.Data);
            var jsonData = jsonResult.Data as object;
            Assert.IsNotNull(jsonData);

            //Assert.AreEqual(jsonData.Name, organisation.Name);
            //Assert.AreEqual(jsonData.Description, organisation.Description);
            //Assert.AreEqual(jsonData.Website, organisation.Website);
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

            _controller.SetupAjaxRequest();

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
            //Assert.AreEqual(pageSize, jsonData.Organisations.PagedListItems.Count());
            //Assert.AreEqual(organisations.Count, jsonData.Organisations.TotalResultCount);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Create_Invalid_Input()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

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
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

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
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Create(new OrganisationCreateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Update_Invalid_Input()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

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
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

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
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Update(new OrganisationUpdateInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Delete_Invalid_Input()
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
        public void Organisation_Delete()
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
        public void Organisation_Delete_Invalid_Permission()
        {
            _mockUserContext.Setup(x => x.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.Delete(new IdInput());

            Assert.IsInstanceOf<HttpUnauthorizedResult>(result);
        }

        #endregion
    }
}