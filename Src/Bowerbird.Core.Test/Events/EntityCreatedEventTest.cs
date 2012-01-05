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

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)] 
        public void EntityCreatedEvent_Constructor_Passing_Null_Entity_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new EntityCreatedEvent<ProxyObjects.ProxyDomainEvent>(null, FakeObjects.TestUser()) ));
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void EntityCreatedEvent_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new EntityCreatedEvent<ProxyObjects.ProxyDomainEvent>(new ProxyObjects.ProxyDomainEvent(), null)));
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)] 
        public void EntityCreatedEvent_Entity_Is_Specified_Generic_Type()
        {
            Assert.IsInstanceOf<ProxyObjects.ProxyDomainEvent>(new EntityCreatedEvent<ProxyObjects.ProxyDomainEvent>(new ProxyObjects.ProxyDomainEvent(), FakeObjects.TestUser()).Entity);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void EntityCreatedEvent_User_Is_A_User()
        {
            Assert.IsInstanceOf<User>(new EntityCreatedEvent<ProxyObjects.ProxyDomainEvent>(new ProxyObjects.ProxyDomainEvent(), FakeObjects.TestUser()).CreatedByUser);
        }

        #endregion

        #region Method tests

        #endregion					
    }
}