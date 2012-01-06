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
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Events;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class DomainModelUpdatedEventTest
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
        public void DomainModelUpdatedEvent_Constructor_Passing_Null_DomainModel_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                        () => new DomainModelUpdatedEvent<ProxyObjects.ProxyDomainEvent>(null, FakeObjects.TestUser())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DomainModelUpdatedEvent_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new DomainModelUpdatedEvent<ProxyObjects.ProxyDomainEvent>(new ProxyObjects.ProxyDomainEvent(), null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DomainModelUpdatedEvent_Constructor_Passing_DomainModel_And_User_Sets_Properties()
        {
            var testDomainEvent = new ProxyObjects.ProxyDomainEvent();
            var testUser = FakeObjects.TestUser();

            var domainModelUpdatedEvent = new DomainModelUpdatedEvent<ProxyObjects.ProxyDomainEvent>(testDomainEvent, testUser);

            Assert.AreEqual(domainModelUpdatedEvent.DomainModel, testDomainEvent);
            Assert.AreEqual(domainModelUpdatedEvent.User, testUser);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void DomainModelUpdatedEvent_DomainModel_Is_Specified_Generic_Type()
        {
            Assert.IsInstanceOf<ProxyObjects.ProxyDomainEvent>(
                new DomainModelUpdatedEvent<ProxyObjects.ProxyDomainEvent>(
                    new ProxyObjects.ProxyDomainEvent(), FakeObjects.TestUser()).DomainModel);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DomainModelCreatedEvent_User_Is_TypeOf_User()
        {
            Assert.IsInstanceOf<User>(
                new DomainModelUpdatedEvent<ProxyObjects.ProxyDomainEvent>(
                    new ProxyObjects.ProxyDomainEvent(), FakeObjects.TestUser()).User);
        }

        #endregion

        #region Method tests

        #endregion
    }
}