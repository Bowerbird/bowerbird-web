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

    using System.Linq;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Extensions;

    #endregion
    
    public class RoleTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static Role TestRole()
        {
            return new Role(FakeValues.KeyString, FakeValues.Name,FakeValues.Description, FakeObjects.TestPermissions());
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Constructor_Passing_Empty_Id_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Role(string.Empty, FakeValues.Name, FakeValues.Description, FakeObjects.TestPermissions())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Constructor_Passing_Empty_Name_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Role(FakeValues.KeyString, string.Empty, FakeValues.Description, FakeObjects.TestPermissions())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Constructor_Passing_Empty_Description_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Role(FakeValues.KeyString, FakeValues.Name, string.Empty, FakeObjects.TestPermissions())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Constructor_Passing_Null_Permissions_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Role(FakeValues.KeyString, FakeValues.Name, FakeValues.Description, null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Constructor_Populates_Properties_With_Values()
        {
            var testRole = new Role(FakeValues.KeyString, FakeValues.Name, FakeValues.Description, FakeObjects.TestPermissions());

            Assert.AreEqual(testRole.Id, FakeValues.KeyString.PrependWith("roles/"));
            Assert.AreEqual(testRole.Name, FakeValues.Name);
            Assert.AreEqual(testRole.Description, FakeValues.Description);
            Assert.AreEqual(testRole.Permissions.Select(x => x.Id).ToList(), FakeObjects.TestPermissions().Select(x => x.Id).ToList());
            Assert.AreEqual(testRole.Permissions.Select(x => x.Name).ToList(), FakeObjects.TestPermissions().Select(x => x.Name).ToList());
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Name_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestRole().Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Description_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestRole().Description);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Role_Permissions_Is_ListOf_DenormalisedNamedDomainModelReference_Permission()
        {
            Assert.IsInstanceOf<List<DenormalisedNamedDomainModelReference<Permission>>>(TestRole().Permissions);
        }

        #endregion

        #region Method tests

        #endregion
    }
}