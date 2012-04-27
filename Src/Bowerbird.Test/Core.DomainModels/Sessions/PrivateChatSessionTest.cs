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
using Bowerbird.Core.DomainModels.Sessions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.Core.DomainModels.Sessions
{
    [TestFixture]
    public class PrivateChatSessionTest
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
        public void PrivateChatSession_Constructor()
        {
            var user = FakeObjects.TestUserWithId();
            var clientId = Guid.NewGuid();
            var chatId = Guid.NewGuid();

            var privateChatSession = new PrivateChatSession(
                user,
                clientId.ToString(),
                chatId.ToString()
                );

            Assert.AreEqual(privateChatSession.User, user.DenormalisedUserReference());
            Assert.AreEqual(privateChatSession.ClientId, clientId.ToString());
            Assert.AreEqual(privateChatSession.ChatId, chatId.ToString());
        }

        #endregion
    }
}
