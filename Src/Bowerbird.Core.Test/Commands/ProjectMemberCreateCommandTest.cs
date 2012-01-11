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
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ProjectMemberCreateCommandTest
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
        public void ProjectMemberCreateCommand_CreatedByUserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectMemberCreateCommand() { CreatedByUserId = FakeValues.KeyString }.CreatedByUserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectMemberCreateCommand() { UserId = FakeValues.KeyString }.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateCommand_ProjectId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectMemberCreateCommand() { ProjectId = FakeValues.KeyString }.ProjectId);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<NotImplementedException>(() =>
                    new ProjectMemberCreateCommand().ValidationResults()));
        }

        #endregion 
    }
}