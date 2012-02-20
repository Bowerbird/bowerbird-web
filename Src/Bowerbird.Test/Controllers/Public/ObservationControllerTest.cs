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
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers.Public;
using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Moq;
using Raven.Client;

namespace Bowerbird.Test.Controllers.Public
{
    #region Namespaces

    #endregion

    [TestFixture]
    public class ObservationControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private ObservationController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.InMemoryDocumentStore();
            _controller = new ObservationController(
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

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Index_In_ViewModel_Format()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(observation);
                session.SaveChanges();
            }

            _controller.SetupFormRequest();

            _controller.Index(new IdInput() { Id = observation.Id });

            Assert.IsInstanceOf<ObservationIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as ObservationIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Observation, observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Index_In_Json_Format()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(observation);
                session.SaveChanges();
            }

            _controller.SetupAjaxRequest();

            var result = _controller.Index(new IdInput() { Id = observation.Id });

            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = result as JsonResult;

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ObservationIndex>(jsonResult.Data);

            var jsonData = jsonResult.Data as ObservationIndex;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(observation, jsonData.Observation);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observations_By_User_In_Json_Format()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();

            const int page = 1;
            const int pageSize = 10;

            var observations = new List<Observation>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);

                for (var i = 0; i < 15; i++)
                {
                    var observation = FakeObjects.TestObservationWithId(i.ToString());
                    
                    observations.Add(observation);
                    session.Store(observation);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new ObservationListInput() { Page = page, PageSize = pageSize, CreatedByUserId = user.Id});

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<ObservationList>(jsonResult.Data);
            var jsonData = jsonResult.Data as ObservationList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);

            foreach (var observation in jsonData.Observations.PagedListItems.Select(x => x.PageObject.User).ToList())
            {
                Assert.IsTrue(observation == user.DenormalisedUserReference());
            }

        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Observations_By_Project_In_Json_Format()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();

            const int page = 1;
            const int pageSize = 10;

            var observations = new List<Observation>();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);

                for (var i = 0; i < 15; i++)
                {
                    var observation = FakeObjects.TestObservationWithId(i.ToString());
                    observation.AddGroupContribution(project, user, FakeValues.CreatedDateTime.AddDays(i*-1));
                    observations.Add(observation);
                    session.Store(observation);
                }

                session.SaveChanges();
            }

            var result = _controller.List(new ObservationListInput() { Page = page, PageSize = pageSize, GroupId = project.Id });

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.IsNotNull(jsonResult.Data);
            Assert.IsInstanceOf<ObservationList>(jsonResult.Data);
            var jsonData = jsonResult.Data as ObservationList;

            Assert.IsNotNull(jsonData);
            Assert.AreEqual(page, jsonData.Page);
            Assert.AreEqual(pageSize, jsonData.PageSize);
            Assert.AreEqual(pageSize, jsonData.Observations.PagedListItems.Count());
            Assert.AreEqual(observations.Count, jsonData.Observations.TotalResultCount);
            Assert.AreEqual(project, jsonData.Project);
        }

        #endregion
    }
}