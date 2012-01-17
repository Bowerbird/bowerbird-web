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
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    public class ProjectMemberCreateCommandHandlerTest
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
        public void ProjectMemberCreateCommandHandler_Constructor_Passing_Null_ProjectMemberRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectMemberCreateCommandHandler(
                        null,
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Project>>().Object,
                        new Mock<IRepository<Role>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateCommandHandler_Constructor_Passing_Null_UserMemberRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectMemberCreateCommandHandler(
                        new Mock<IRepository<ProjectMember>>().Object,
                        null,
                        new Mock<IRepository<Project>>().Object,
                        new Mock<IRepository<Role>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateCommandHandler_Constructor_Passing_Null_ProjectRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectMemberCreateCommandHandler(
                        new Mock<IRepository<ProjectMember>>().Object,
                        new Mock<IRepository<User>>().Object,
                        null,
                        new Mock<IRepository<Role>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateCommandHandler_Constructor_Passing_Null_RoleRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectMemberCreateCommandHandler(
                        new Mock<IRepository<ProjectMember>>().Object,
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Project>>().Object,
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            var commandHandler = new ProjectMemberCreateCommandHandler(
                new Mock<IRepository<ProjectMember>>().Object,
                new Mock<IRepository<User>>().Object,
                new Mock<IRepository<Project>>().Object,
                new Mock<IRepository<Role>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ProjectMemberCreateCommandHandler_Handle_Creates_ProjectMember()
        {
            ProjectMember result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<ProjectMember>(session);
                var proxyRepository = new ProxyRepository<ProjectMember>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockProjectRepository = new Mock<IRepository<Project>>();
                var mockRoleRepository = new Mock<IRepository<Role>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                mockProjectRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestProject);

                mockRoleRepository
                    .Setup(x => x.Load(It.IsAny<List<string>>()))
                    .Returns(FakeObjects.TestRoles);

                var projectMemberCreateCommandHandler = new ProjectMemberCreateCommandHandler(
                    proxyRepository,
                    mockUserRepository.Object,
                    mockProjectRepository.Object,
                    mockRoleRepository.Object
                    );

                projectMemberCreateCommandHandler.Handle(new ProjectMemberCreateCommand()
                {
                    ProjectId = FakeValues.KeyString,
                    CreatedByUserId = FakeValues.UserId,
                    Roles = FakeValues.StringList,
                    UserId = FakeValues.UserId
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion 
    }
}