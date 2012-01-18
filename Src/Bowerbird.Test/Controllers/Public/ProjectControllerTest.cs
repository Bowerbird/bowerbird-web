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
using Bowerbird.Core.DomainModels;
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
    public class ProjectControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private ProjectController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();
            _controller = new ProjectController(_documentStore.OpenSession());
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Index_Returns_ProjectIndex_ViewModel_With_Project_Having_Observations()
        {
            var project = FakeObjects.TestProjectWithId();
            var observation1 = FakeObjects.TestObservationWithId();
            var observation2 = FakeObjects.TestObservationWithId(FakeValues.KeyString.AppendWith(FakeValues.KeyString));
            var projectobservation1 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation1);
            var projectobservation2 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation2);

            using (var session = _documentStore.OpenSession())
            {
                session.Store(observation1);
                session.Store(observation2);
                session.Store(project);
                session.Store(projectobservation1);
                session.Store(projectobservation2);

                session.SaveChanges();
            }

            _controller.Index(new IdInput() { Id = FakeValues.KeyString.PrependWith("projects/") });

            Assert.IsInstanceOf<ProjectIndex>(_controller.ViewData.Model);

            var viewModel = _controller.ViewData.Model as ProjectIndex;

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Project, project);
            Assert.IsTrue(viewModel.Observations.Contains(observation1));
            Assert.IsTrue(viewModel.Observations.Contains(observation2));
        }

        #endregion
    }
}