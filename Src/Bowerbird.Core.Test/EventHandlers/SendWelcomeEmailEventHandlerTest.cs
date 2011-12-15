using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Core.Test.EventHandlers
{
    [TestFixture]
    public class SendWelcomeEmailEventHandlerTest
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


        #endregion

        #region Constructor tests


        #endregion

        #region Property tests


        #endregion

        #region Method tests

        [Test]
        public void SendWelcomeEmailEventHandler_Handle_Passing_Null_UserCreatedEvent_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () =>
                    new SendWelcomeEmailEventHandler()
                    .Handle(null)
                ));
        }

        [Test, Ignore]
        public void SendWelcomeEmailEventHandler_Handle_Passing_UserCreatedEvent_DoesStuffYetDefined()
        {
            
        }

        #endregion					

    }
}