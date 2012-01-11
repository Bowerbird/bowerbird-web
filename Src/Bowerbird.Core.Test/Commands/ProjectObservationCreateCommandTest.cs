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
    public class ProjectObservationCreateCommandTest
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

        #region Property Tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationCreateCommand_ProjectId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectObservationCreateCommand(){ProjectId = FakeValues.KeyString}.ProjectId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationCreateCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectObservationCreateCommand() { UserId = FakeValues.KeyString }.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationCreateCommand_ObservationId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProjectObservationCreateCommand() { ObservationId = FakeValues.KeyString }.ObservationId);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectObservationCreateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<NotImplementedException>(()=>
                    new ProjectObservationCreateCommand().ValidationResults()));
        }

        #endregion
    }
}