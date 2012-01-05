/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
    public class EntityUpdatedEventTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void EntityUpdatedEvent_Constructor_Passing_Null_Entity_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                        () => new EntityUpdatedEvent<ProxyObjects.ProxyDomainEvent>(null, FakeObjects.TestUser())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void EntityUpdatedEvent_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new EntityUpdatedEvent<ProxyObjects.ProxyDomainEvent>(new ProxyObjects.ProxyDomainEvent(), null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void EntityUpdatedEvent_Constructor_Passing_Entity_And_User_Sets_Properties()
        {
            var testDomainEvent = new ProxyObjects.ProxyDomainEvent();
            var testUser = FakeObjects.TestUser();

            var entityUpdatedEvent = new EntityUpdatedEvent<ProxyObjects.ProxyDomainEvent>(testDomainEvent, testUser);

            Assert.AreEqual(entityUpdatedEvent.Entity, testDomainEvent);
            Assert.AreEqual(entityUpdatedEvent.User, testUser);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void EntityUpdatedEvent_Entity_Is_Specified_Generic_Type()
        {
            Assert.IsInstanceOf<ProxyObjects.ProxyDomainEvent>(
                new EntityUpdatedEvent<ProxyObjects.ProxyDomainEvent>(
                    new ProxyObjects.ProxyDomainEvent(), FakeObjects.TestUser()).Entity);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void EntityCreatedEvent_User_Is_TypeOf_User()
        {
            Assert.IsInstanceOf<User>(
                new EntityUpdatedEvent<ProxyObjects.ProxyDomainEvent>(
                    new ProxyObjects.ProxyDomainEvent(), FakeObjects.TestUser()).User);
        }

        #endregion

        #region Method tests

        #endregion
    }
}