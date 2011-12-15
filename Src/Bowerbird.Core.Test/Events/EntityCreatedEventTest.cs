using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Events;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Core.Test.Events
{
    [TestFixture]
    public class EntityCreatedEventTest
    {

        #region Infrastructure

        [SetUp]
        public void TestInitialize()
        {

        }

        [TearDown]
        public void TestCleanup()
        {

        }

        #endregion

        #region Helpers

        private class ProxyDomainEvent : IEntity
        {
            public string Id
            {
                get { return FakeValues.KeyString; }
            }
        }

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
        public void EntityCreatedEvent_Constructor_Passing_Null_Entity_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => new EntityCreatedEvent<ProxyDomainEvent>(null, TestUser()) ));
        }

        [Test]
        public void EntityCreatedEvent_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => new EntityCreatedEvent<ProxyDomainEvent>(new ProxyDomainEvent(), null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        #endregion					
				
    }
}