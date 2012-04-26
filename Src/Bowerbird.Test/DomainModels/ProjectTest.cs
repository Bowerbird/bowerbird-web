/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using System;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class ProjectTest
    {
        #region Test Infrastructure

        const string additionalString = "_";

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
        public void Project_Constructor()
        {
            var createdDateTime = DateTime.UtcNow;

            var testProject = new Project(
                FakeObjects.TestUserWithId(), 
                FakeValues.Name, 
                FakeValues.Description,
                FakeValues.Website,
                null,
                createdDateTime
                );

            Assert.AreEqual(testProject.Name, FakeValues.Name);
            Assert.AreEqual(testProject.Description, FakeValues.Description);
            Assert.AreEqual(testProject.Website, FakeValues.Website);
            Assert.AreEqual(testProject.CreatedDateTime, createdDateTime);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_UpdateDetails()
        {
            var createdDateTime = DateTime.UtcNow;

            var testProject = new Project(
                FakeObjects.TestUserWithId(),
                FakeValues.Name,
                FakeValues.Description,
                FakeValues.Website,
                null,
                createdDateTime
                );

            testProject.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Name.AppendWith(additionalString),
                FakeValues.Description.AppendWith(additionalString),
                FakeValues.Website.AppendWith(additionalString),
                null
                );

            Assert.AreEqual(testProject.Name, FakeValues.Name.AppendWith(additionalString));
            Assert.AreEqual(testProject.Description, FakeValues.Description.AppendWith(additionalString));
            Assert.AreEqual(testProject.Website, FakeValues.Website.AppendWith(additionalString));
        }

        #endregion
    }
}