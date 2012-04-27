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
using NUnit.Framework;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
    public class OrganisationTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        const string additionalString = "_";

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Constructor()
        {
            var createdDateTime = DateTime.UtcNow;

            var testOrganisation = new Organisation(
                FakeObjects.TestUserWithId(), 
                FakeValues.Name, 
                FakeValues.Description, 
                FakeValues.Website,
                null,
                createdDateTime);

            Assert.AreEqual(testOrganisation.Name, FakeValues.Name);
            Assert.AreEqual(testOrganisation.Description, FakeValues.Description);
            Assert.AreEqual(testOrganisation.Website, FakeValues.Website);
            Assert.AreEqual(testOrganisation.CreatedDateTime, createdDateTime);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails()
        {
            var createdDateTime = DateTime.UtcNow;

            var testOrganisation = new Organisation(
                FakeObjects.TestUserWithId(), 
                FakeValues.Name, 
                FakeValues.Description,
                FakeValues.Website,
                null,
                createdDateTime);

            testOrganisation.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Name.AppendWith(additionalString),
                FakeValues.Description.AppendWith(additionalString),
                FakeValues.Website.AppendWith(additionalString),
                null);

            Assert.AreEqual(testOrganisation.Name, FakeValues.Name.AppendWith(additionalString));
            Assert.AreEqual(testOrganisation.Description, FakeValues.Description.AppendWith(additionalString));
            Assert.AreEqual(testOrganisation.Website, FakeValues.Website.AppendWith(additionalString));
            Assert.AreEqual(testOrganisation.CreatedDateTime, createdDateTime);
        }

        #endregion
    }
}