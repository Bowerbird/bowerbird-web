/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Test.ViewModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ProjectPostCreateInputTest
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
        public void ProjectPostCreateInput_ProjectId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateInput() { ProjectId = FakeValues.KeyString }.ProjectId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateInput_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateInput() { UserId = FakeValues.KeyString }.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateInput_Subject_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateInput() { Subject = FakeValues.Subject }.Subject);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateInput_Message_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectPostCreateInput() { Message = FakeValues.Message }.Message);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateInput_Timestamp_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(new ProjectPostCreateInput() { Timestamp = FakeValues.CreatedDateTime }.Timestamp);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateInput_MediaResources_Is_TypeOf_List_String()
        {
            Assert.IsInstanceOf<IEnumerable<string>>(new ProjectPostCreateInput() { MediaResources = new List<string>() }.MediaResources);
        }

        #endregion

        #region Method tests

        #endregion
    }
}