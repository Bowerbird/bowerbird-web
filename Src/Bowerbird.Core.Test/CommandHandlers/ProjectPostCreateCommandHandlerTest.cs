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
    using Bowerbird.Core.DomainModels.Posts;

    #endregion

    public class ProjectPostCreateCommandHandlerTest
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
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_ProjectRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        null,
                        new Mock<IRepository<ProjectPost>>().Object,
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<MediaResource>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_ProjectPostRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        new Mock<IRepository<Project>>().Object,
                        null,
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<MediaResource>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        new Mock<IRepository<Project>>().Object,
                        new Mock<IRepository<ProjectPost>>().Object,
                        null,
                        new Mock<IRepository<MediaResource>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_MediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        new Mock<IRepository<Project>>().Object,
                        new Mock<IRepository<ProjectPost>>().Object,
                        new Mock<IRepository<User>>().Object,
                        null
                        )));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        new Mock<IRepository<Project>>().Object,
                        new Mock<IRepository<ProjectPost>>().Object,
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<MediaResource>>().Object)
                        .Handle(null)));
        }


        [Test]
        [Category(TestCategory.Persistance)]
        public void ObservationCommentCreateCommandHandler_Handle_Creates_ObservationComment()
        {
            ProjectPost result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<ProjectPost>(session);
                var proxyRepository = new ProxyRepository<ProjectPost>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockProjectRepository = new Mock<IRepository<Project>>();
                var mockMediaResourceRepository = new Mock<IRepository<MediaResource>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                mockProjectRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestProject);

                mockMediaResourceRepository
                    .Setup(x => x.Load(It.IsAny<List<string>>()))
                    .Returns(new List<MediaResource>(){new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat,FakeValues.Description)});

                var projectPostCreateCommandHandler = new ProjectPostCreateCommandHandler(
                    mockProjectRepository.Object,
                    proxyRepository,
                    mockUserRepository.Object,
                    mockMediaResourceRepository.Object
                    );

                projectPostCreateCommandHandler.Handle(new ProjectPostCreateCommand()
                {
                    UserId = FakeValues.UserId,
                    MediaResources = FakeValues.StringList,
                    Message = FakeValues.Message,
                    ProjectId = FakeValues.KeyString,
                    Subject = FakeValues.Subject,
                    Timestamp = FakeValues.CreatedDateTime
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion 
    }
}