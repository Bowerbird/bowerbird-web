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

    #endregion

    [TestFixture]
    public class UserCreateCommandHandlerTest
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
        public void UserCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new UserCreateCommandHandler(
                        null, 
                        new Mock<IRepository<Role>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommandHandler_Constructor_Passing_Null_RoleRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => new 
                    UserCreateCommandHandler(
                        new Mock<IRepository<User>>().Object, 
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommandHandler_Handle_Passing_Null_Command_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new UserCreateCommandHandler(
                        new Mock<IRepository<User>>().Object, 
                        new Mock<IRepository<Role>>().Object)
                        .Handle(null)));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void UserCreateCommandHandler_Handle_Creates_User()
        {
            User result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<User>(session);
                var proxyRepository = new ProxyRepository<User>(repository);
                var mockRoleRepository = new Mock<IRepository<Role>>();
                
                proxyRepository.NotifyOnAdd(x => result = x);

                mockRoleRepository
                    .Setup(x => x.Load(It.IsAny<List<string>>()))
                    .Returns(FakeObjects.TestRoles);

                var userCreateCommandHandler = new UserCreateCommandHandler(
                    proxyRepository,
                    mockRoleRepository.Object
                    );

                userCreateCommandHandler.Handle(new UserCreateCommand()
                {
                    Roles = FakeValues.StringList,
                    Description = FakeValues.Description,
                    Email = FakeValues.Email,
                    FirstName = FakeValues.FirstName,
                    LastName = FakeValues.LastName,
                    Password = FakeValues.Password
                }); 

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion 
    }
}