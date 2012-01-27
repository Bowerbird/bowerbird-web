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
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers.Public;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Public
{
    [TestFixture]
    public class OrganisationControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private OrganisationController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _controller = new OrganisationController(_documentStore.OpenSession());
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Index_NonAjaxCall_Returns_ViewModel_OrganisationIndex_Having_Organisation()
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

            var viewModel = _controller.ViewData.Model as OrganisationIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Organisation, organisation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Index_AjaxCall_Returns_Json_OrganisationIndex_Having_Organisation()
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

            Assert.AreEqual(jsonData.Organisation, organisation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_List_Returns_OrganisationList_In_Json_Format()
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

        #endregion
    }
}