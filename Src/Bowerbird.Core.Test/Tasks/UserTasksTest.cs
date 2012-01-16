/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Repositories;
using Bowerbird.Core.Test.ProxyRepositories;
using Moq;

namespace Bowerbird.Core.Test.Tasks
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Extensions;
    using Bowerbird.Core.Tasks;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class UserTasksTest
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

        [Test, Category(TestCategory.Unit)]
        public void UserTasks_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserTasks(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Integration), Category(TestCategory.Persistance)]
        public void UserTasks_AreCredentialsValid_Passing_Valid_Email_And_Password_Returns_True()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(FakeObjects.TestUser());

                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<User>(session);
                var proxyRepository = new ProxyUserRepository(repository);
                Assert.IsTrue(new UserTasks(proxyRepository).AreCredentialsValid(FakeValues.Email, FakeValues.Password));
            }
        }

        [Test, Category(TestCategory.Integration), Category(TestCategory.Persistance)]
        public void UserTasks_AreCredentialsValid_Passing_InValid_Username_And_Password_Returns_True()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(FakeObjects.TestUser());

                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<User>(session);
                var proxyRepository = new ProxyUserRepository(repository);
                Assert.IsFalse(new UserTasks(proxyRepository).AreCredentialsValid(FakeValues.KeyString.AppendWith("blah"), FakeValues.Password));
            }
        }

        [Test, Category(TestCategory.Integration), Category(TestCategory.Persistance)]
        public void UserTasks_GetEmailByResetPasswordKey_Passing_Empty_ResetPasswordKey_Throws_DesignByContractException()
        {
            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserTasks(new Mock<IRepository<User>>().Object).GetEmailByResetPasswordKey(string.Empty)));
            }
        }

        [Test, Category(TestCategory.Integration), Category(TestCategory.Persistance)]
        public void UserTasks_GetEmailByResetPasswordKey_Passing_Valid_ResetPasswordKey_Returns_Email()
        {
            string resetPasswordKey = null;

            using (var session = _store.OpenSession())
            {
                var user = FakeObjects.TestUser();
                user.RequestPasswordReset();

                resetPasswordKey = user.ResetPasswordKey;

                session.Store(user);

                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<User>(session);
                var proxyRepository = new ProxyUserRepository(repository);

                string result = new UserTasks(proxyRepository).GetEmailByResetPasswordKey(resetPasswordKey);

                Assert.AreEqual(FakeValues.Email, result);
            }
        }

        #endregion	

    }
}