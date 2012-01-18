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

        #region Method tests

        #endregion
    }
}