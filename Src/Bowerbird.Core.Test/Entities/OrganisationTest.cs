/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Entities
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
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
        public void Organisation_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Organisation(null, FakeValues.Name, FakeValues.Description, FakeValues.Website)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Constructor_Passing_Empty_Name_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Organisation(FakeObjects.TestUser(), string.Empty, FakeValues.Description, FakeValues.Website)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Constructor_Passing_Empty_Description_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Organisation(FakeObjects.TestUser(), FakeValues.Name, string.Empty, FakeValues.Website)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Organisation_Constructor_Populates_Property_Values()
        {
            var testOrganisation = new Organisation(FakeObjects.TestUser(), FakeValues.Name, FakeValues.Description, FakeValues.Website);

            Assert.AreEqual(testOrganisation.Name, FakeValues.Name);
            Assert.AreEqual(testOrganisation.Description, FakeValues.Description);
            Assert.AreEqual(testOrganisation.Website, FakeValues.Website);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Description_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestOrganisation().Description);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Name_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestOrganisation().Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_Website_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestOrganisation().Website);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationNote_UpdateDetails_Updates_Property_Values()
        {
            var testOrganisation = new Organisation(FakeObjects.TestUser(), FakeValues.Name, FakeValues.Description, FakeValues.Website);

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