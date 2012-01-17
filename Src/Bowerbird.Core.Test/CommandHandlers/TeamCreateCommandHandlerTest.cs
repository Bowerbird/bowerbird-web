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
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.Test.ProxyRepositories;

    #endregion

    [TestFixture]
    public class TeamCreateCommandHandlerTest
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
        public void TeamCreateCommandHandler_Constructor_Passing_Null_TeamRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new TeamCreateCommandHandler(null, new Mock<IRepository<User>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamCreateCommandHandler(new Mock<IRepository<Team>>().Object,null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void TeamCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new TeamCreateCommandHandler(
                        new Mock<IRepository<Team>>().Object,
                        new Mock<IRepository<User>>().Object)
                        .Handle(null)
                        ));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void TeamCreateCommandHandler_Handle_Creates_Team()
        {
            Team result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<Team>(session);
                var proxyRepository = new ProxyRepository<Team>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                var teamCreateCommandHandler = new TeamCreateCommandHandler(
                    proxyRepository,
                    mockUserRepository.Object
                    );

                teamCreateCommandHandler.Handle(new TeamCreateCommand()
                {
                    UserId = FakeValues.UserId,
                    Description = FakeValues.Description,
                    Name = FakeValues.Name,
                    Website = FakeValues.Website
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion
    }
}