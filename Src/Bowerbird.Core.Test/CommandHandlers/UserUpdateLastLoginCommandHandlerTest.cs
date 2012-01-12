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
using Raven.Client;

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using System;
    using System.Linq;
    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class UserUpdateLastLoginCommandHandlerTest
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

        private UserUpdateLastLoginCommandHandler TestUserUpdateLastLoginCommandHandler(IDocumentSession documentSession)
        {
            var repository = new Repository<User>(documentSession);
            var proxyUserRepository = new ProxyUserRepository(repository);
            return new UserUpdateLastLoginCommandHandler(proxyUserRepository);
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserUpdateLastLoginCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserUpdateLastLoginCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserUpdateLastLoginCommandHandler_Handle_Passing_Null_UserUpdateLastLoginCommand_Throws_DesignByContractException()
        {
            var userUpdateLastLoginCommandHandler = new UserUpdateLastLoginCommandHandler(new Mock<IRepository<User>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    userUpdateLastLoginCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void UserUpdateLastLoginCommandHandler_Handle_Passing_UserUpdateLastLoginCommand_Updates_User_LastLoggedIn_Property()
        {
            DateTime originalValue;
            DateTime newValue;

            using (var session = _store.OpenSession())
            {
                var user = FakeObjects.TestUserWithId();
                user.UpdateLastLoggedIn();
                originalValue = user.LastLoggedIn;

                session.Store(user);

                session.SaveChanges();

                var userUpdateLastLoginCommandHandler = TestUserUpdateLastLoginCommandHandler(session);

                userUpdateLastLoginCommandHandler.Handle(new UserUpdateLastLoginCommand()
                       {
                           Email = FakeValues.Email
                       });

                session.SaveChanges();

                newValue = session.Load<User>("users/" + FakeValues.UserId).LastLoggedIn;
            }

            Assert.AreNotEqual(originalValue, newValue);
            Assert.Greater(newValue, originalValue);
        }

        #endregion 
    }
}