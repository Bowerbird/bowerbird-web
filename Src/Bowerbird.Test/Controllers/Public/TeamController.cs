/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers.Public;
using Bowerbird.Web.ViewModels.Public;
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
            _documentStore = DocumentStoreHelper.TestDocumentStore();

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
        public void Team_List_Returns_Json_Success()
        {
            var result = _controller.List(null, null, null);

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Success");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_Index_NonAjaxCall_Returns_TeamIndex_Json_Having_Projects()
        {
            var team = FakeObjects.TestTeam();
            var project = FakeObjects.TestProjectWithId();
            project.Team = team;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(team);
                session.Store(project);
                session.SaveChanges();
            }

            _controller.Index(new IdInput() { Id = FakeValues.KeyString.PrependWith("teams/") });

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
            var team = FakeObjects.TestTeam();
            var project = FakeObjects.TestProjectWithId();
            project.Team = team;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(team);
                session.Store(project);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Index(new IdInput() { Id = FakeValues.KeyString.PrependWith("projects/") });

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

        #endregion 
    }
}