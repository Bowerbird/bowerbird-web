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
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ProjectPostUpdateCommandTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize()
        {
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostUpdateCommand() {UserId = FakeValues.UserId}.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommand_Id_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostUpdateCommand() { Id = FakeValues.KeyString }.Id);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommand_Message_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostUpdateCommand() { Message = FakeValues.Message }.Message);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommand_Subject_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostUpdateCommand() { Subject = FakeValues.Subject }.Subject);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommand_TimeStamp_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(new ProjectPostUpdateCommand() { Timestamp = FakeValues.CreatedDateTime }.Timestamp);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommand_MediaResources_Is_TypeOf_List_String()
        {
            Assert.IsInstanceOf<List<string>>(new ProjectPostUpdateCommand() { MediaResources = new List<string>() }.MediaResources);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<NotImplementedException>(() =>
                    new ProjectPostUpdateCommand().ValidationResults()));
        }


        #endregion
    }
}   