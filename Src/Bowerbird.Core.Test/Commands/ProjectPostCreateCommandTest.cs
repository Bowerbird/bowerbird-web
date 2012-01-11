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

    [TestFixture]
    public class ProjectPostCreateCommandTest
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
        public void ProjectPostCreateCommand_ProjectId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateCommand() { ProjectId = FakeValues.KeyString }.ProjectId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateCommand() { UserId = FakeValues.KeyString }.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommand_Subject_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateCommand() { Subject = FakeValues.Subject }.Subject);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommand_Message_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateCommand() { Message = FakeValues.Message }.Message);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommand_Timestamp_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(new ProjectPostCreateCommand() {Timestamp = FakeValues.CreatedDateTime}.Timestamp);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<NotImplementedException>(() =>
                new ProjectPostCreateCommand().ValidationResults()));
        }

        #endregion
    }
}