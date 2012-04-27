/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
    public class UserProjectTest
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

        [Test]
        [Category(TestCategory.Unit)]
        public void UserProject_Constructor()
        {
            var createdDateTime = DateTime.UtcNow;

            var testTeam = new UserProject(
                FakeObjects.TestUserWithId(), 
                createdDateTime
                );

            Assert.AreEqual(testTeam.Name, "User Group");
            Assert.AreEqual(testTeam.CreatedDateTime, createdDateTime);
        }

        #endregion
    }
}