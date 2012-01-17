/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Test.ProxyRepositories;

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;

    #endregion

    public class ProjectObservationCreateCommandHandlerTest
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
        public void ProjectObservationCreateCommandHandler_Constructor_Passing_Null_ProjectObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectObservationCreateCommandHandler(
                        null,
                        new Mock<IRepository<Project>>().Object,
                        new Mock<IRepository<Observation>>().Object,
                        new Mock<IRepository<User>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationCreateCommandHandler_Constructor_Passing_Null_ProjectRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectObservationCreateCommandHandler(
                        new Mock<IRepository<ProjectObservation>>().Object,
                        null,
                        new Mock<IRepository<Observation>>().Object,
                        new Mock<IRepository<User>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationCreateCommandHandler_Constructor_Passing_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectObservationCreateCommandHandler(
                        new Mock<IRepository<ProjectObservation>>().Object,
                        new Mock<IRepository<Project>>().Object,
                        null,
                        new Mock<IRepository<User>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectObservationCreateCommandHandler(
                        new Mock<IRepository<ProjectObservation>>().Object,
                        new Mock<IRepository<Project>>().Object,
                        new Mock<IRepository<Observation>>().Object,
                        null
                        )));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            var commandHandler = new ProjectObservationCreateCommandHandler(
                new Mock<IRepository<ProjectObservation>>().Object,
                new Mock<IRepository<Project>>().Object,
                new Mock<IRepository<Observation>>().Object,
                new Mock<IRepository<User>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ProjectObservationCreateCommandHandler_Handle_Creates_ProjectObservation()
        {
            ProjectObservation result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<ProjectObservation>(session);
                var proxyRepository = new ProxyRepository<ProjectObservation>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockProjectRepository = new Mock<IRepository<Project>>();
                var mockObservationRepository = new Mock<IRepository<Observation>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                mockProjectRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestProject);

                mockObservationRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestObservationWithId);

                var projectObservationCreateCommandHandler = new ProjectObservationCreateCommandHandler(
                    proxyRepository,
                    mockProjectRepository.Object,
                    mockObservationRepository.Object,
                    mockUserRepository.Object
                    );

                projectObservationCreateCommandHandler.Handle(new ProjectObservationCreateCommand()
                {
                    CreatedDateTime = FakeValues.CreatedDateTime,
                    ObservationId = FakeValues.KeyString,
                    UserId = FakeValues.UserId,
                    ProjectId = FakeValues.KeyString
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion 
    }
}