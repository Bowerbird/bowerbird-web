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
    public class ProjectCreateInputTest
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
        public void ProjectCreateInput_Name_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ProjectCreateInput() { Name = FakeValues.Name }.Name );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateInput_Description_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ProjectCreateInput() { Description = FakeValues.Description }.Description);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateInput_UserId_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ProjectCreateInput() { UserId = FakeValues.UserId }.UserId);
        }

        #endregion

        #region Method tests

        #endregion
    }
}