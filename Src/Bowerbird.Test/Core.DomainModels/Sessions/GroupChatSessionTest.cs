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
    public class GroupChatSessionTest
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
        public void GroupChatSession_Constructor()
        {
            var user = FakeObjects.TestUserWithId();
            var clientId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;

            var groupChatSession = new GroupChatSession(
                user,
                clientId.ToString(),
                groupId.ToString()
                );

            Assert.AreEqual(groupChatSession.GroupId, groupId.ToString());
            Assert.AreEqual(groupChatSession.ClientId, clientId.ToString());
            Assert.AreEqual(groupChatSession.User, user.DenormalisedUserReference());
            Assert.AreEqual(groupChatSession.Status, (int)Connection.ConnectionStatus.Online);
        }

        #endregion
    }
}