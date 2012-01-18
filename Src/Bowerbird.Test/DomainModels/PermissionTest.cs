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

        #endregion

        #region Constructor tests

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

        #region Method tests

        #endregion
    }
}