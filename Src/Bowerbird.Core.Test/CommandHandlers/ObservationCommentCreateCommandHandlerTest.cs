/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels.Comments;
    using Bowerbird.Core.Test.ProxyRepositories;

    #endregion

    [TestFixture]
    public class ObservationCommentCreateCommandHandlerTest
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
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new ObservationCommentCreateCommandHandler(
                        null, 
                        new Mock<IRepository<Observation>>().Object,
                        new Mock<IRepository<ObservationComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null,
                        new Mock<IRepository<ObservationComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_ObservationCommentRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentCreateCommandHandler(
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
        public void ObservationCommentCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            var commandHandler = new ObservationCommentCreateCommandHandler(
                new Mock<IRepository<User>>().Object,
                new Mock<IRepository<Observation>>().Object,
                new Mock<IRepository<ObservationComment>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ObservationCommentCreateCommandHandler_Handle_Creates_ObservationComment()
        {
            ObservationComment result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<ObservationComment>(session);
                var proxyRepository = new ProxyRepository<ObservationComment>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockObservationRepository = new Mock<IRepository<Observation>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                mockObservationRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestObservationWithId);

                var observationCommentCreateCommandHandler = new ObservationCommentCreateCommandHandler(
                    mockUserRepository.Object,
                    mockObservationRepository.Object,
                    proxyRepository
                    );

                observationCommentCreateCommandHandler.Handle(new ObservationCommentCreateCommand()
                {
                    Comment = FakeValues.Comment,
                    CommentedOn = FakeValues.CreatedDateTime,
                    ObservationId = FakeValues.KeyString,
                    UserId = FakeValues.UserId
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion
    }
}