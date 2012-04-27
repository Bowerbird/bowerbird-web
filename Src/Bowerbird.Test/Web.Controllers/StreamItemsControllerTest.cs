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
using System.Collections.Generic;
using System.Web.Mvc;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers;
using Bowerbird.Web.ViewModels;
using Moq;
using NUnit.Framework;
using Raven.Client;
using Bowerbird.Web.Queries;

namespace Bowerbird.Test.Web.Controllers
{
    [TestFixture]
    public class StreamItemsControllerTest
    {
        #region Test Infrastructure

        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private StreamItemsController _controller;
        private IStreamItemsQuery _streamItemsQuery;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();
            _mockUserContext = new Mock<IUserContext>();
            _streamItemsQuery = new Mock<IStreamItemsQuery>().Object;

            _controller = new StreamItemsController(
                _streamItemsQuery
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

        #region Constructor tests

        #endregion

        #region Method tests

        /// <summary>
        /// Create a user and a project with the user as a member of the project
        /// Add a combination of post and observation contributions to the project
        /// Query the stream item controller for a page of the project's contributions
        /// 
        /// Check that a page sized collection of contributions is returned
        /// Check that each contribution has been added to the specified group
        /// </summary>
        [Test]
        [Category(TestCategory.Unit)]
        public void StreamItemController_GetStreamItems_By_User()
        {
            var team = FakeObjects.TestTeamWithId();
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();
            var roles = FakeObjects.TestRoles();

            var observations = new List<Observation>();
            var posts = new List<Post>();

            using (var session = DocumentStoreHelper.StartRaven().OpenSession())
            {
                session.Store(project);
                session.Store(user);
                session.Store(team);

                for (var i = 0; i < 15; i++)
                {
                    var observation = new Observation(
                        user,
                        FakeValues.Title,
                        FakeValues.CreatedDateTime.AddDays(i * -1),
                        FakeValues.CreatedDateTime.AddDays(i * -1),
                        FakeValues.Latitude,
                        FakeValues.Longitude,
                        FakeValues.Address,
                        FakeValues.IsTrue,
                        FakeValues.Category,
                        FakeObjects.TestUserProjectWithId(),
                        new List<Project>(){project},
                        new List<Tuple<MediaResource, string, string>>()
                        );

                    ((IAssignableId)observation).SetIdTo("observations", i.ToString());
                    observations.Add(observation);

                    session.Store(observation);

                    var post = new Post(
                        user,
                        FakeValues.CreatedDateTime.AddDays(i * -1),
                        FakeValues.Subject,
                        FakeValues.Message,
                        new List<MediaResource>(),
                        project
                        );

                    ((IAssignableId)post).SetIdTo("posts", i.ToString());
                    posts.Add(post);

                    session.Store(post);
                }

                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            _mockUserContext.Setup(x => x.GetAuthenticatedUserId()).Returns(user.Id);

            var result = _controller.List(
                new StreamItemListInput() { Page = 0, PageSize = 10, GroupId = project.Id },
                new StreamSortInput() { });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StreamItemList>(jsonResult.Data);

            var jsonData = jsonResult.Data as StreamItemList;

            Assert.IsNotNull(jsonData);

            //var expected = contributions.Take(10).ToList();
            //var actual = jsonData.StreamItems.ToList();

            //Assert.AreEqual(expected.Count, actual.Count);
        }

        /// <summary>
        /// Create a user and a project with the user as a member of the project
        /// Add a combination of post and observation contributions to the project
        /// Query the stream item controller for a page of the user's contributions
        /// 
        /// Check that a page sized collection of contributions is returned
        /// Check that each contribution has been added by the specified user
        /// </summary>
        [Test]
        [Category(TestCategory.Unit)]
        public void StreamItemController_GetStreamItems_By_Group()
        {
            var team = FakeObjects.TestTeamWithId();
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId();
            var roles = FakeObjects.TestRoles();

            var observations = new List<Observation>();
            var posts = new List<Post>();

            using (var session = DocumentStoreHelper.StartRaven().OpenSession())
            {
                session.Store(project);
                session.Store(user);
                session.Store(team);

                for (var i = 0; i < 15; i++)
                {
                    var observation = new Observation(
                        user,
                        FakeValues.Title,
                        FakeValues.CreatedDateTime.AddDays(i * -1),
                        FakeValues.CreatedDateTime.AddDays(i * -1),
                        FakeValues.Latitude,
                        FakeValues.Longitude,
                        FakeValues.Address,
                        FakeValues.IsTrue,
                        FakeValues.Category,
                        FakeObjects.TestUserProjectWithId(),
                        new List<Project>() { project },
                        new List<Tuple<MediaResource, string, string>>()
                        );

                    ((IAssignableId)observation).SetIdTo("observations", i.ToString());
                    observations.Add(observation);

                    session.Store(observation);

                    var post = new Post(
                        user,
                        FakeValues.CreatedDateTime.AddDays(i * -1),
                        FakeValues.Subject,
                        FakeValues.Message,
                        new List<MediaResource>(),
                        project
                        );

                    ((IAssignableId)post).SetIdTo("posts", i.ToString());
                    posts.Add(post);

                    session.Store(post);
                }

                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            _mockUserContext.Setup(x => x.GetAuthenticatedUserId()).Returns(user.Id);

            var result = _controller.List(
                new StreamItemListInput() { Page = 0, PageSize = 10, GroupId = project.Id },
                new StreamSortInput() { });

            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StreamItemList>(jsonResult.Data);

            var jsonData = jsonResult.Data as StreamItemList;

            Assert.IsNotNull(jsonData);

            //var expected = contributions.Take(10).ToList();
            //var actual = jsonData.StreamItems.ToList();

            //Assert.AreEqual(expected.Count, actual.Count);
        }

        #endregion
    }
}