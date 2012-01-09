/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Commands
{
    #region Namespaces

    using System;

    using NUnit.Framework;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class UserUpdateLastLoginCommandTest
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

        [Test]
        [Category(TestCategory.Unit)]
        public void UserUpdateLastLoginCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new UserUpdateLastLoginCommand() { Email = FakeValues.Email }.Email );
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserUpdateLastLoginCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<NotImplementedException>(() => new UserUpdateLastLoginCommand().ValidationResults()));
        }

        #endregion 
    }
}