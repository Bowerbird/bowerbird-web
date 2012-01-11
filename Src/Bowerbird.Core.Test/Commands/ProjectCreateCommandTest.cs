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
    public class ProjectCreateCommandTest
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
        public void ProjectCreateCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectCreateCommand() { UserId = FakeValues.UserId }.UserId );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommand_Name_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectCreateCommand() { Name = FakeValues.Name }.Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommand_Description_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectCreateCommand() { Description = FakeValues.Description }.Description);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<NotImplementedException>(()=>
                    new ProjectCreateCommand().ValidationResults()
                ));
        }


        #endregion
    }
}