using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Tasks;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Core.Test.Tasks
{
    [TestFixture]
    public class UserTasksTest
    {

        #region Infrastructure

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

        #region Helpers

        /// <summary>
        /// Id: "abc"
        /// Password: "password"
        /// Email: "padil@padil.gov.au"
        /// FirstName: "first name"
        /// LastName: "last name"
        /// Description: "description"
        /// Roles: "Member"
        /// </summary>
        /// <returns></returns>
        private static User TestUser()
        {
            return new User(
                FakeValues.KeyString,
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                FakeValues.Description,
                TestRoles()
            )
            .UpdateLastLoggedIn()
            .UpdateResetPasswordKey()
            .IncrementFlaggedItemsOwned()
            .IncrementFlagsRaised();
        }

        private static IEnumerable<Role> TestRoles()
        {
            return new List<Role>()
            {
                new Role
                (
                    "Member",
                    "Member role",
                    "Member description",
                    TestPermissions()
                )
            };
        }

        private static IEnumerable<Permission> TestPermissions()
        {
            return new List<Permission>
            {
                new Permission("Read", "Read permission", "Read description"),
                new Permission("Write", "Write permission", "Write description")
            };

        }

        #endregion

        #region Constructor tests

        [Test]
        public void UserTasks_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new UserTasks(null)));
        }

        #endregion

        #region Property tests


        #endregion

        #region Method tests

        [Test]
        public void UserTasks_AreCredentialsValid_Passing_Empty_Username_Throws_DesignByContractException()
        {
            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () =>
                        new UserTasks(session)
                            .AreCredentialsValid(
                                string.Empty,
                                FakeValues.Password
                            )));
            }
        }

        [Test]
        public void UserTasks_AreCredentialsValid_Passing_Empty_Password_Throws_DesignByContractException()
        {
            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () =>
                        new UserTasks(session)
                            .AreCredentialsValid(
                                FakeValues.UserName,
                                string.Empty
                            )));
            }
        }

        [Test]
        public void UserTasks_AreCredentialsValid_Passing_Valid_Username_And_Password_Returns_True()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(TestUser());

                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(
                    new UserTasks(session)
                    .AreCredentialsValid(FakeValues.KeyString, FakeValues.Password)
                );
            }
        }

        [Test]
        public void UserTasks_AreCredentialsValid_Passing_InValid_Username_And_Password_Returns_True()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(TestUser());

                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                Assert.IsFalse(
                    new UserTasks(session)
                    .AreCredentialsValid(FakeValues.KeyString.AppendWith("blah"), FakeValues.Password)
                );
            }
        }

        [Test]
        public void UserTasks_IsEmailAvailable_Passing_Empty_Email_Throws_DesignByContractException()
        {
            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () =>
                        new UserTasks(session)
                            .IsEmailAvailable(
                                string.Empty
                            )));
            }
        }

        [Test]
        public void UserTasks_IsEmailAvailable_Passing_Available_Email_Returns_True()
        {
            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(
                    new UserTasks(session)
                    .IsEmailAvailable(FakeValues.Email)
                );
            }
        }

        [Test]
        public void UserTasks_IsEmailAvailable_Passing_Existing_Email_Returns_False()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(TestUser());

                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                Assert.IsFalse(
                    new UserTasks(session)
                    .IsEmailAvailable(FakeValues.Email)
                );
            }
        }

        #endregion	

    }
}