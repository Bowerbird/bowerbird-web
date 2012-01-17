/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.Test.ProxyRepositories;
using Raven.Client;

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ObservationNoteCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
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
        public void ObservationNoteCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationNoteCreateCommandHandler(
                        null,
                        new Mock<IRepository<Observation>>().Object,
                        new Mock<IRepository<ObservationNote>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNoteCreateCommandHandler_Constructor_Passing_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationNoteCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null,
                        new Mock<IRepository<ObservationNote>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNoteCreateCommandHandler_Constructor_Passing_Null_ObservationNoteRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationNoteCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Observation>>().Object,
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNoteCreateCommandHandler_Handle_Passing_Null_ObservationNoteCreate_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationNoteCreateCommandHandler(
                        new Mock<IRepository<User>>().Object, 
                        new Mock<IRepository<Observation>>().Object,
                        new Mock<IRepository<ObservationNote>>().Object)
                        .Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ObservationCommentCreateCommandHandler_Handle_Creates_ObservationComment()
        {
            ObservationNote result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<ObservationNote>(session);
                var proxyRepository = new ProxyRepository<ObservationNote>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockObservationRepository = new Mock<IRepository<Observation>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                mockObservationRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestObservationWithId);

                var observationNoteCreateCommandHandler = new ObservationNoteCreateCommandHandler(
                    mockUserRepository.Object,
                    mockObservationRepository.Object,
                    proxyRepository
                    );

                observationNoteCreateCommandHandler.Handle(new ObservationNoteCreateCommand()
                {
                    CommonName = FakeValues.CommonName,
                    ScientificName = FakeValues.ScientificName,
                    Taxonomy = FakeValues.Taxonomy,
                    Tags = FakeValues.Tags,
                    SubmittedOn = FakeValues.CreatedDateTime,
                    Notes = FakeValues.Notes
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion
    }
}