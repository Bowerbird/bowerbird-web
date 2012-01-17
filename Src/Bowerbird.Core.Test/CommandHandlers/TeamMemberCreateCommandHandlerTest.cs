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

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Test.ProxyRepositories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    [TestFixture]
    public class TeamMemberCreateCommandHandlerTest
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
        public void TeamMemberCreateCommandHandler_Constructor_Passing_Null_TeamRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamMemberCreateCommandHandler(
                        null, 
                        new Mock<IRepository<TeamMember>>().Object, 
                        new Mock<IRepository<User>>().Object, 
                        new Mock<IRepository<Role>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamMemberCreateCommandHandler_Constructor_Passing_Null_TeamMemberRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamMemberCreateCommandHandler(
                        new Mock<IRepository<Team>>().Object,
                        null,
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Role>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamMemberCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamMemberCreateCommandHandler(
                        new Mock<IRepository<Team>>().Object,
                        new Mock<IRepository<TeamMember>>().Object,
                        null,
                        new Mock<IRepository<Role>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamMemberCreateCommandHandler_Constructor_Passing_Null_RoleRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamMemberCreateCommandHandler(
                        new Mock<IRepository<Team>>().Object,
                        new Mock<IRepository<TeamMember>>().Object,
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
        public void TeamMemberCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamMemberCreateCommandHandler(
                        new Mock<IRepository<Team>>().Object,
                        new Mock<IRepository<TeamMember>>().Object,
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Role>>().Object
                        ).Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void TeamMemberCreateCommandHandler_Handle_Creates_TeamMember()
        {
            TeamMember result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<TeamMember>(session);
                var proxyRepository = new ProxyRepository<TeamMember>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockTeamRepository = new Mock<IRepository<Team>>();
                var mockRoleRepository = new Mock<IRepository<Role>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                mockTeamRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestTeam);

                mockRoleRepository
                    .Setup(x => x.Load(It.IsAny<List<string>>()))
                    .Returns(FakeObjects.TestRoles);

                var teamMemberCreateCommandHandler = new TeamMemberCreateCommandHandler(
                    mockTeamRepository.Object,
                    proxyRepository,
                    mockUserRepository.Object,
                    mockRoleRepository.Object
                    );

                teamMemberCreateCommandHandler.Handle(new TeamMemberCreateCommand()
                {
                    UserId = FakeValues.UserId,
                    CreatedByUserId = FakeValues.UserId,
                    Roles = FakeValues.StringList,
                    TeamId = FakeValues.KeyString
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion
    }
}