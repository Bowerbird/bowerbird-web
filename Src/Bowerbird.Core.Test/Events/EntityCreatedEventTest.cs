/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Events
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Events;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] 
    public class EntityCreatedEventTest
    {
        #region Test Infrastructure

        [SetUp] 
        public void TestInitialize(){ }

        [TearDown] 
        public void TestCleanup(){ }

        #endregion

        #region Test Helpers

        private class ProxyDomainEvent : IDomainEvent
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

        [Test, Category(TestCategories.Unit)] 
        public void EntityCreatedEvent_Constructor_Passing_Null_Entity_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new EntityCreatedEvent<ProxyDomainEvent>(null, TestUser()) ));
        }

        [Test, Category(TestCategories.Unit)] 
        public void EntityCreatedEvent_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new EntityCreatedEvent<ProxyDomainEvent>(new ProxyDomainEvent(), null)));
        }

        #endregion

        #region Property tests

        [Test, Category(TestCategories.Unit)] 
        public void EntityCreatedEvent_Entity_Is_Specified_Generic_Type()
        {
            Assert.IsInstanceOf<ProxyDomainEvent>(new EntityCreatedEvent<ProxyDomainEvent>(new ProxyDomainEvent(), TestUser()).Entity);
        }

        [Test, Category(TestCategories.Unit)]
        public void EntityCreatedEvent_User_Is_A_User()
        {
            Assert.IsInstanceOf<User>(new EntityCreatedEvent<ProxyDomainEvent>(new ProxyDomainEvent(), TestUser()).CreatedByUser);
        }

        #endregion

        #region Method tests

        #endregion					
    }
}