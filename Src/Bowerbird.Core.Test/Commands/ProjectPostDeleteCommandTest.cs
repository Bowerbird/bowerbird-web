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

    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    public class ProjectPostDeleteCommandTest
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
        public void ProjectPostDeleteCommand_Id_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostDeleteCommand() { Id = FakeValues.KeyString }.Id );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostDeleteCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostDeleteCommand() { UserId = FakeValues.UserId }.UserId);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostDeleteCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<NotImplementedException>(() =>
                    new ProjectPostDeleteCommand().ValidationResults()));
        }

        #endregion 
    }
}