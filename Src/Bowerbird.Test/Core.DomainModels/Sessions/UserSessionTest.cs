/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Sessions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.Core.DomainModels.Sessions
{
    [TestFixture]
    public class UserSessionTest
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
        public void UserSession_Constructor()
        {
            var user = FakeObjects.TestUserWithId();
            var clientId = Guid.NewGuid();

            var userSession = new UserSession(
                user,
                clientId.ToString()
                );

            Assert.AreEqual(userSession.User, user.DenormalisedUserReference());
            Assert.AreEqual(userSession.ClientId, clientId.ToString());
            Assert.AreEqual(userSession.Status, (int)Connection.ConnectionStatus.Online);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserSession_UpdateDetails()
        {
            var user = FakeObjects.TestUserWithId();
            var clientId = Guid.NewGuid();

            var userSession = new UserSession(
                user,
                clientId.ToString()
                );

            userSession.UpdateDetails((int)Connection.ConnectionStatus.Away);

            Assert.AreEqual(userSession.Status, (int)Connection.ConnectionStatus.Away);
        }

        #endregion
    }
}