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

    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Events;
    using Bowerbird.Core.DomainModels;

    #endregion

    [TestFixture]
    public class UserLoggedInEventTest
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
        public void UserLoggedInEvent_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserLoggedInEvent(null)));
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserLoggedInEvent_User_Is_TypeOf_User()
        {
            Assert.IsInstanceOf<User>(new UserLoggedInEvent(FakeObjects.TestUser()).User);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}