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
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ProjectMemberCreateInputTest
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
        public void ProjectMemberCreateInput_CreatedByUserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectMemberCreateInput() { CreatedByUserId = FakeValues.UserId }.CreatedByUserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateInput_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectMemberCreateInput() { UserId = FakeValues.KeyString }.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateInput_ProjectId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectMemberCreateInput() { ProjectId = FakeValues.KeyString }.ProjectId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberCreateInput_Roles_Is_TypeOf_List_String()
        {
            Assert.IsInstanceOf<List<string>>(new ProjectMemberCreateInput() { Roles = FakeObjects.TestRoles().Select(x => x.Name).ToList() }.Roles);
        }

        #endregion

        #region Method tests

        #endregion
    }
}