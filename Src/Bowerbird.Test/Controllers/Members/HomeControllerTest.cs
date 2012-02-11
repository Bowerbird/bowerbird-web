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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Members;
using Bowerbird.Web.ViewModels.Members;
using Moq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace Bowerbird.Test.Controllers.Members
{
    [TestFixture]
    public class HomeControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private HomeController _controller;

        [SetUp]
        public void TestInitialize()
        {
            //_documentStore = DocumentStoreHelper.ServerDocumentStore();
            _documentStore = DocumentStoreHelper.InMemoryDocumentStore();

            _mockCommandProcessor = new Mock<ICommandProcessor>();

            _mockUserContext = new Mock<IUserContext>();

            _controller = new HomeController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                //_documentStore.OpenSession(DocumentStoreHelper.TestDb)
                _documentStore.OpenSession()
                );
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

        /// <summary>
        /// Create a user, team and project
        /// Add the user as a group member to the team and project
        /// 
        /// Query the Home Controller for the index passing a user's id, page number and page size
        /// 
        /// Check that the response contains:
        /// The user's profile
        /// A list of menu items containing the project
        /// A list of menu items containing the team
        /// A list of menu items containing the watchlist
        /// </summary>
        [Test]
        [Category(TestCategory.Unit)]
        public void HomeController_Index()
        {
            var team = FakeObjects.TestTeamWithId();
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();
            var roles = FakeObjects.TestRoles();

            var watchlist = new Watchlist(
                user,
                FakeValues.Name,
                FakeValues.QuerystringJson
                );

            var contributions = new List<Contribution>();

            var teamMember = new GroupMember(
                user,
                team,
                user,
                roles
                );

            ((IAssignableId)teamMember).SetIdTo("teammembers", "abcd");

            var projectMember = new GroupMember(
                user,
                project,
                user,
                roles
                );

            ((IAssignableId)projectMember).SetIdTo("projectmembers", "efgh");

            using (var session = _documentStore.OpenSession())
            {
                session.Store(project);
                session.Store(user);
                session.Store(team);
                session.Store(teamMember);
                session.Store(projectMember);
                session.Store(watchlist);

                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            _mockUserContext.Setup(x => x.GetAuthenticatedUserId()).Returns(user.Id);

            var result = _controller.Index(new HomeIndexInput() { UserId = user.Id, Page = 1, PageSize = 10 });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<HomeIndex>(jsonResult.Data);

            var jsonData = jsonResult.Data as HomeIndex;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(user.Id, jsonData.UserProfile.Id);
            Assert.AreEqual(string.Format("{0} {1}", user.FirstName, user.LastName), jsonData.UserProfile.Name);
            Assert.IsTrue(jsonData.TeamMenu.Count() == 1);
            Assert.AreEqual(team.Name, jsonData.TeamMenu.ToList()[0].Name);
            Assert.AreEqual(team.Id, jsonData.TeamMenu.ToList()[0].Id);
            Assert.IsTrue(jsonData.ProjectMenu.Count() == 1);
            Assert.AreEqual(project.Name, jsonData.ProjectMenu.ToList()[0].Name);
            Assert.AreEqual(project.Id, jsonData.ProjectMenu.ToList()[0].Id);
            Assert.IsTrue(jsonData.WatchlistMenu.Count() == 1);
            Assert.AreEqual(watchlist.Name, jsonData.WatchlistMenu.ToList()[0].Name);
        }

        #endregion
    }
}