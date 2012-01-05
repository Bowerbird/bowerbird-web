/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Entities
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Core.Entities;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;

    #endregion

    [TestFixture, Ignore]
    public class ValueObjectTest
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

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ValueObject_Equality_Operator_Comparing_Two_Equal_Objects_Returns_True()
        {
            var leftValueObject = new ProxyObjects.ProxyValueObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());
            var rightValueObject = new ProxyObjects.ProxyValueObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());

            //Assert.IsTrue(leftValueObject.HasSameObjectSignatureAs(rightValueObject));
            var leftHashCode = leftValueObject.GetHashCode();
            var rightHashCode = rightValueObject.GetHashCode();

            Assert.AreNotEqual(leftHashCode, rightHashCode);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void BaseObject_Equality_Operator_Comparing_Two_Equal_Objects_Returns_True()
        {
            var leftValueObject = new ProxyObjects.ProxyBaseObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());
            var rightValueObject = new ProxyObjects.ProxyBaseObject(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestTeam());

            Assert.IsTrue(leftValueObject.Equals(rightValueObject));
            //var leftHashCode = leftValueObject.GetHashCode();
            //var rightHashCode = rightValueObject.GetHashCode();

            //Assert.AreNotEqual(leftHashCode, rightHashCode);
        }

        #endregion 
    }
}