/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Posts;

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
    public class TeamPostCreateCommandHandlerTest
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
        public void TeamPostCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamPostCreateCommandHandler(
                        null, 
                        new Mock<IRepository<Team>>().Object,
                        new Mock<IRepository<TeamPost>>().Object,
                        new Mock<IRepository<MediaResource>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPostCreateCommandHandler_Constructor_Passing_Null_TeamRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamPostCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null,
                        new Mock<IRepository<TeamPost>>().Object,
                        new Mock<IRepository<MediaResource>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPostCreateCommandHandler_Constructor_Passing_Null_TeamPostRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamPostCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Team>>().Object,
                        null,
                        new Mock<IRepository<MediaResource>>().Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPostCreateCommandHandler_Constructor_Passing_Null_MediaResource_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamPostCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Team>>().Object,
                        new Mock<IRepository<TeamPost>>().Object,
                        null
                        )));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamPostCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                   BowerbirdThrows.Exception<DesignByContractException>(() =>
                       new TeamPostCreateCommandHandler(
                           new Mock<IRepository<User>>().Object,
                           new Mock<IRepository<Team>>().Object,
                           new Mock<IRepository<TeamPost>>().Object,
                           new Mock<IRepository<MediaResource>>().Object
                           ).Handle(null)
                           ));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void TeamPostCreateCommandHandler_Handle_Creates_TeamPost()
        {
            TeamPost result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<TeamPost>(session);
                var proxyRepository = new ProxyRepository<TeamPost>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockTeamRepository = new Mock<IRepository<Team>>();
                var mockMediaResourceRepository = new Mock<IRepository<MediaResource>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                mockTeamRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestTeam);

                mockMediaResourceRepository
                    .Setup(x => x.Load(It.IsAny<List<string>>()))
                    .Returns(new List<MediaResource>(){new ProxyObjects.ProxyMediaResource(FakeValues.Filename,FakeValues.FileFormat, FakeValues.Description)});

                var teamPostCreateCommandHandler = new TeamPostCreateCommandHandler(
                    mockUserRepository.Object,
                    mockTeamRepository.Object,
                    proxyRepository,
                    mockMediaResourceRepository.Object
                    );

                teamPostCreateCommandHandler.Handle(new TeamPostCreateCommand()
                {
                    UserId = FakeValues.UserId,
                    MediaResources = FakeValues.StringList,
                    Message = FakeValues.Message,
                    PostedOn = FakeValues.CreatedDateTime,
                    Subject = FakeValues.Subject,
                    TeamId = FakeValues.KeyString
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }


        #endregion
    }
}