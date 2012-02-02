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
using System.ComponentModel.Composition;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Members;
using Bowerbird.Web.Indexes;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Moq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using Raven.Client.Embedded;
using Raven.Abstractions.Indexing;

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
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _mockUserContext = new Mock<IUserContext>();

            _controller = new HomeController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object,
                _documentStore.OpenSession()
                );

            IndexCreation.CreateIndexes(typeof(StreamItem_ByParentId).Assembly, _documentStore);
            IndexCreation.CreateIndexes(typeof(ProjectMember_ByUserId).Assembly, _documentStore);
            IndexCreation.CreateIndexes(typeof(TeamMember_ByUserId).Assembly, _documentStore);
            IndexCreation.CreateIndexes(typeof(User_ByUserId).Assembly, _documentStore);
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

        //[Test]
        //[Category(TestCategory.Persistance)]
        //public void Home_Index_Returns_HomeIndex_Containing_MenuItems_User_And_StreamItems()
        //{
        //    var user = FakeObjects.TestUserWithId();
        //    var project = FakeObjects.TestProjectWithId();
        //    var team = FakeObjects.TestTeamWithId();
        //    var projectMember = FakeObjects.TestProjectMemberWithId("abc");
        //    var teamMember = FakeObjects.TestTeamMemberWithId("def");
        //    var streamItems = new List<StreamItemViewModel>();

        //    const int page = 1;
        //    const int pageSize = 10;

        //    using (var session = _documentStore.OpenSession())
        //    {
        //        session.Store(user);
        //        session.Store(project);
        //        session.Store(team);
        //        session.Store(projectMember);
        //        session.Store(teamMember);
        //        session.SaveChanges();

        //        for (var i = 0; i < 10; i++)
        //        {
        //            var observation = FakeObjects.TestObservationWithId(i.ToString());
        //            var observationPost = FakeObjects.TestObservationNoteWithId(i.ToString());
        //            var post = FakeObjects.TestPostWithId(i.ToString());
        //            session.Store(observation);
        //            session.Store(observationPost);
        //            session.Store(post);
        //            var streamItem1 = new StreamItem(user, DateTime.Now.AddDays(i * -1), "Observation", observation.Id, observation);
        //            var streamItem2 = new StreamItem(user, DateTime.Now.AddDays(i * -1), "ObservationPost", observationPost.Id, observationPost);
        //            var streamItem3 = new StreamItem(user, DateTime.Now.AddDays(i * -1), "ProjectPost", post.Id, post);
        //            streamItems.Add(new StreamItemViewModel() { Item = observation, SubmittedOn = streamItem1.CreatedDateTime, Type = "Observation" });
        //            streamItems.Add(new StreamItemViewModel() { Item = observationPost, SubmittedOn = streamItem2.CreatedDateTime, Type = "ObservationPost" });
        //            streamItems.Add(new StreamItemViewModel() { Item = post, SubmittedOn = streamItem3.CreatedDateTime, Type = "ProjectPost" });
        //        }

        //        session.SaveChanges();
        //    }

        //    _controller.SetupAjaxRequest();

        //    var result = _controller.Index(new HomeIndexInput() { Page = page, PageSize = pageSize, UserId = user.Id});

        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<JsonResult>(result);

        //    var jsonResult = result as JsonResult;
        //    Assert.IsNotNull(jsonResult);

        //    Assert.IsNotNull(jsonResult.Data);
        //    Assert.IsInstanceOf<HomeIndex>(jsonResult.Data);
        //    var jsonData = jsonResult.Data as HomeIndex;

        //    Assert.IsNotNull(jsonData);
        //    Assert.AreEqual(page, jsonData.Page);
        //    Assert.AreEqual(pageSize, jsonData.PageSize);
        //    Assert.AreEqual(1, jsonData.ProjectMenu.Count());
        //    Assert.AreEqual(1, jsonData.TeamMenu.Count());
        //    Assert.AreEqual(streamItems.Count, jsonData.StreamItems.TotalResultCount);
        //}

        #endregion 
    }
}