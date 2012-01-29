/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Team Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Members;
using Bowerbird.Web.ViewModels.Members;
using NUnit.Framework;
using Moq;
using Bowerbird.Test.Utils;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Members
{
    [TestFixture]
    public class TeamMemberControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private TeamMemberController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
            _documentStore = DocumentStoreHelper.TestDocumentStore();

            _controller = new TeamMemberController(
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
        public void TeamMember_List_Returns_TeamMemberList_In_Json_Format()
        {
            var user = FakeObjects.TestUserWithId();
            var team = FakeObjects.TestTeamWithId();
            var teamMembers = new List<TeamMember>();

            const int page = 1;
            const int pageSize = 10;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(team);
                session.SaveChanges();

                for (var i = 0; i < 15; i++)
                {
                    var teamMember = FakeObjects.TestTeamMemberWithId(i.ToString());
                    teamMembers.Add(teamMember);
                    session.Store(teamMember);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new TeamMemberListInput() { Page = page, PageSize = pageSize, TeamId = team.Id, UserId = user.Id });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsInstanceOf<TeamMemberList>(jsonResult.Data);
            var jsonData = jsonResult.Data as TeamMemberList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.TeamMembers.PagedListItems.Count());
            Assert.AreEqual(teamMembers.Count, jsonData.TeamMembers.TotalResultCount);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamMember_Create_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Create(new TeamMemberCreateInput()
            {
                UserId = FakeValues.UserId,
                TeamId = FakeValues.KeyString,
                Roles = FakeValues.StringList
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamMember_Create_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Create(new TeamMemberCreateInput()
            {
                UserId = FakeValues.UserId,
                TeamId = FakeValues.KeyString,
                Roles = FakeValues.StringList
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamMember_Delete_Passing_Invalid_Input_Returns_Json_Error()
        {
            _controller.ModelState.AddModelError("Error", "Error");

            var result = _controller.Delete(new TeamMemberDeleteInput()
            {
                TeamId = FakeValues.KeyString,
                TeamMemberId = FakeValues.KeyString
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data, "Failure");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamMember_Delete_Passing_Valid_Input_Returns_Json_Success()
        {
            var result = _controller.Delete(new TeamMemberDeleteInput()
            {
                TeamId = FakeValues.KeyString,
                TeamMemberId = FakeValues.KeyString
            });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(jsonResult.Data.ToString().ToLower(), "Success".ToLower());
        }

        #endregion
    }
}