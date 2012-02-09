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
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers.Public;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Public
{
    [TestFixture]
    public class TeamControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private TeamController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.InMemoryDocumentStore();

            _controller = new TeamController(
                _documentStore.OpenSession()
                );
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Index_As_ViewModel()
        {
            var team = FakeObjects.TestTeamWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(team);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Index(new IdInput() { Id = team.Id });

            Assert.IsInstanceOf<TeamIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as TeamIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Team, team);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Index_As_Json()
        {
            var team = FakeObjects.TestTeamWithId();

            using (var session = _documentStore.OpenSession())
            {
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
        }

        #endregion 
    }
}