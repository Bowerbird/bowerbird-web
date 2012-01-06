/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DomainModels
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Extensions;
    using Bowerbird.Test.Utils;

    #endregion

    public class PermissionTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static Permission TestPermission()
        {
            return new Permission(FakeValues.KeyString, FakeValues.Name, FakeValues.Description);
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Permission_Constructor_Passing_Empty_Id_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Permission(string.Empty, FakeValues.Name, FakeValues.Description)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Permission_Constructor_Passing_Empty_Name_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Permission(FakeValues.KeyString, string.Empty, FakeValues.Description)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Permission_Constructor_Passing_Empty_Description_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Permission(FakeValues.KeyString, FakeValues.Name, string.Empty)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Permission_Constructor_Populates_Properties_With_Values()
        {
            var testPermission = new Permission(FakeValues.KeyString, FakeValues.Name, FakeValues.Description);

            Assert.AreEqual(testPermission.Description, FakeValues.Description);
            Assert.AreEqual(testPermission.Name, FakeValues.Name);
            Assert.AreEqual(testPermission.Id, FakeValues.KeyString.PrependWith("permissions/"));
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Permission_Id_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestPermission().Id);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Permission_Name_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestPermission().Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Permission_Description_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestPermission().Description);
        }

        #endregion

        #region Method tests

        #endregion
    }
}