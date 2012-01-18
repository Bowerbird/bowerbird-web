/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.Hubs
{
    #region Namespaces

    using System;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Test.Utils;
    using Bowerbird.Web.Hubs;

    #endregion

    [TestFixture] 
    public class ActivityHubTest
    {
        #region Test Infrastructure

        private Mock<IDocumentSession> _mockDocumentSession;

        [SetUp] 
        public void TestInitialize() 
        {
            _mockDocumentSession = new Mock<IDocumentSession>();
        }

        [TearDown] 
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor Tests

        #endregion

        #region Property Tests

        #endregion

        #region Method Tests

        [Test, Category(TestCategory.Unit)] 
        public void ActivityHub_StartActivityStream_Throws_NotImplementedException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<NotImplementedException>(() => new ActivityHub(_mockDocumentSession.Object).StartActivityStream()));
        }

        #endregion
    }
}