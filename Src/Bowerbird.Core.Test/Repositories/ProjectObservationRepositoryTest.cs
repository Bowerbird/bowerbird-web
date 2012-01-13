/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Repositories
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;

    #endregion

    [TestFixture]
    public class ProjectObservationRepositoryTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;
        private ProjectObservationRepository _repository;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store.Dispose();

            _store = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void ProjectObservationRepository_Can_Save_And_Load_ProjectObservation()
        {
            var write = new ProjectObservation(
                FakeObjects.TestUser(),
                FakeValues.CreatedDateTime,
                FakeObjects.TestProject(),
                FakeObjects.TestObservation()
                );

            _repository = new ProjectObservationRepository(_store.OpenSession());

            _repository.Add(write);

            _repository.SaveChanges();

            var read = _repository.Load(FakeObjects.TestProject().Id, FakeObjects.TestObservation().Id);

            Assert.AreEqual(write, read);
        }

        #endregion
    }
}