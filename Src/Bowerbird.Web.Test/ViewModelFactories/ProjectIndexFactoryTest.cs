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

namespace Bowerbird.Web.Test.ViewModelFactories
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Moq;
    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.ViewModelFactories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.MediaResources;
    using Bowerbird.Core.Extensions;

    #endregion

    [TestFixture]
    public class ProjectIndexFactoryTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;
        private Mock<IPagedListFactory> _mockPagedListFactory;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
            _mockPagedListFactory = new Mock<IPagedListFactory>();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectIndexFactory_Constructor_Passing_Null_PagedListFactory_Throws_DesignByContractException()
        {
            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(() =>
                        new ProjectIndexFactory(
                            session,
                            _mockPagedListFactory.Object
                            )));
            }
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Integration)]
        public void ProjectIndexFactory_Make_Returns_ProjectIndex_With_Project_And_Observations()
        {
            ProjectIndex projectIndex;

            var project = FakeObjects.TestProjectWithId();
            var observation1 = FakeObjects.TestObservationWithId();
            var observation2 = FakeObjects.TestObservationWithId(FakeValues.KeyString.AppendWith(FakeValues.KeyString));
            var projectobservation1 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation1);
            var projectobservation2 = new ProjectObservation(FakeObjects.TestUser(), DateTime.Now, project, observation2);

            using (var session = _store.OpenSession())
            {
                session.Store(observation1);
                session.Store(observation2);
                session.Store(project);
                session.Store(projectobservation1);
                session.Store(projectobservation2);

                session.SaveChanges();

                projectIndex = new ProjectIndexFactory(session, new Mock<IPagedListFactory>().Object)
                    .Make(new ProjectIndexInput(){ProjectId = project.Id});
            }

            Assert.IsNotNull(projectIndex);
            Assert.AreEqual(projectIndex.Project, project);
            Assert.IsTrue(projectIndex.Observations.Contains(observation1));
            Assert.IsTrue(projectIndex.Observations.Contains(observation2));
        }

        #endregion 
    }
}