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
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class GroupChatMessageTest
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
        public void GroupChatMessage_Constructor()
        {
            var user = FakeObjects.TestUserWithId();
            var group = FakeObjects.TestProjectWithId();
            var timestamp = DateTime.UtcNow;

            var groupChatMessage = new GroupChatMessage(
                user,
                group,
                user,
                timestamp,
                FakeValues.Message
                );

            Assert.AreEqual(groupChatMessage.GroupId, group.Id);
            Assert.AreEqual(groupChatMessage.Message, FakeValues.Message);
            Assert.AreEqual(groupChatMessage.TargetUser, user.DenormalisedUserReference());
            Assert.AreEqual(groupChatMessage.Timestamp, timestamp);
        }

        #endregion
    }
}