/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using System;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class PrivateChatMessageTest
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
        public void PrivateChatMessage_Constructor()
        { 
            var currentDateTime = DateTime.UtcNow;

            var privateChatMessage = new PrivateChatMessage(
                FakeObjects.TestUserWithId(),
                FakeValues.KeyString,
                FakeObjects.TestUserWithId(),
                FakeValues.Message,
                currentDateTime
                );

            Assert.AreEqual(FakeObjects.TestUserWithId().DenormalisedUserReference(), privateChatMessage.User);
            Assert.AreEqual(FakeValues.KeyString, privateChatMessage.ChatId);
            Assert.AreEqual(FakeObjects.TestUserWithId().DenormalisedUserReference(), privateChatMessage.TargetUser);
            Assert.AreEqual(FakeValues.Message, privateChatMessage.Message);
            Assert.AreEqual(currentDateTime, privateChatMessage.Timestamp);
        }

        #endregion
    }
}
