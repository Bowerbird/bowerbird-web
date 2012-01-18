/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.EventHandlers
{
    #region Namespaces

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Web.Config;
    using Bowerbird.Web.EventHandlers;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] 
    public class NotifyActivityEventHandlerBaseTest
    {
        #region Test Infrastructure

        private Mock<IUserContext> _mockUserContext;

        [SetUp] 
        public void TestInitialize() {  _mockUserContext = new Mock<IUserContext>(); }

        [TearDown] 
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        /// <summary>
        /// Access to abstract NotifyActivityEventHandlerBase and protected methods via proxy subclass
        /// </summary>
        private class ProxyNotifyActivityEventHandler : NotifyActivityEventHandlerBase 
        { 
            public ProxyNotifyActivityEventHandler(IUserContext userContext):base(userContext){}

            public new void Notify(string type, User user, object data)
            {
                base.Notify(type, user, data);
            }
        }

        #endregion

        #region Constructor Tests

        [Test, Category(TestCategory.Unit)] 
        public void NotifyActivityEventHandler_Constructor_Passing_Null_UserContext_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyNotifyActivityEventHandler(null)));
        }

        #endregion

        #region Property Tests

        #endregion

        #region Method Tests

        [Test,Ignore]
        [Category(TestCategory.Integration)] 
        public void NotifyActivityEventHandler_Notify_Calls_UserContext_GetChannel()
        { 
            var clients = new
            {
                activityOccurred = "string"
            };

            _mockUserContext.Setup(x => x.GetChannel()).Returns(clients).Verifiable();

            new ProxyNotifyActivityEventHandler(_mockUserContext.Object).Notify(FakeValues.ActivityType, FakeObjects.TestUser(), new object());

            _mockUserContext.Verify();
        }

        #endregion
    }
}