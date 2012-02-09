/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.DomainModels
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.Extensions;

    #endregion

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

        private static Organisation TestOrganisation()
        {
            return new Organisation(FakeObjects.TestUser(), FakeValues.Name, FakeValues.Description, FakeValues.Website);
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Constructor()
        {
            var testOrganisation = new Organisation(
                FakeObjects.TestUser(), 
                FakeValues.Name, 
                FakeValues.Description, 
                FakeValues.Website);

            Assert.AreEqual(testOrganisation.Name, FakeValues.Name);
            Assert.AreEqual(testOrganisation.Description, FakeValues.Description);
            Assert.AreEqual(testOrganisation.Website, FakeValues.Website);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails()
        {
            var testOrganisation = new Organisation(
                FakeObjects.TestUser(), 
                FakeValues.Name, 
                FakeValues.Description, 
                FakeValues.Website);

            testOrganisation.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Name.AppendWith(additionalString),
                FakeValues.Description.AppendWith(additionalString),
                FakeValues.Website.AppendWith(additionalString));

            Assert.AreEqual(testOrganisation.Name, FakeValues.Name.AppendWith(additionalString));
            Assert.AreEqual(testOrganisation.Description, FakeValues.Description.AppendWith(additionalString));
            Assert.AreEqual(testOrganisation.Website, FakeValues.Website.AppendWith(additionalString));
        }

        #endregion
    }
}