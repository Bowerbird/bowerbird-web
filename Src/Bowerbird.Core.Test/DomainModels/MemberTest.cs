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

    using System;
    using System.Linq;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;

    #endregion

    public class MemberTest
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
        public void Member_Constructor_ByProxy_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyObjects.ProxyMember(null, FakeObjects.TestRoles())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_Constructor_ByProxy_Passing_Null_Roles_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyObjects.ProxyMember(FakeObjects.TestUser(), null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_Constructor_ByProxy_Sets_Property_Values()
        {
            var testUser = FakeObjects.TestUser();
            var testRoles = FakeObjects.TestRoles();
            var testMember = new ProxyObjects.ProxyMember(testUser, testRoles);

            Assert.AreEqual(testMember.Roles.Select(x => x.Id).ToList(), testRoles.Select(x => x.Id).ToList());
            Assert.AreEqual(testMember.Roles.Select(x => x.Name).ToList(), testRoles.Select(x => x.Name).ToList());
            Assert.AreEqual(testMember.User.FirstName, testUser.FirstName);
            Assert.AreEqual(testMember.User.LastName, testUser.LastName);
            Assert.AreEqual(testMember.User.Id, testUser.Id);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_User_Is_TypeOf_DenormalisedUserReference()
        {
            var testMember = new ProxyObjects.ProxyMember(FakeObjects.TestUser(), FakeObjects.TestRoles());

            Assert.IsInstanceOf<DenormalisedUserReference>(testMember.User);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_Roles_Is_TypeOf_DenormalisedNamedDomainModelReference_Role_List()
        {
            var testMember = new ProxyObjects.ProxyMember(FakeObjects.TestUser(), FakeObjects.TestRoles());

            Assert.IsInstanceOf<List<DenormalisedNamedDomainModelReference<Role>>>(testMember.Roles);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_AddRole_Adds_Role_To_Member()
        {
            var testRole = new Role(FakeValues.KeyString, FakeValues.Name, FakeValues.Description, FakeObjects.TestPermissions());
            var testMember = new ProxyObjects.ProxyMember(FakeObjects.TestUser(), FakeObjects.TestRoles());
            
            testMember.AddRole(testRole);
            var addedRole = testMember.Roles.Where(x => x.Id == testRole.Id).FirstOrDefault();

            Assert.IsNotNull(addedRole);
            Assert.AreEqual(testRole.Id, addedRole.Id);
            Assert.AreEqual(testRole.Name, addedRole.Name);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_RemoveRole_Removes_Role_From_Member()
        {
            var testRole = new Role(FakeValues.KeyString, FakeValues.Name, FakeValues.Description, FakeObjects.TestPermissions());
            var testMember = new ProxyObjects.ProxyMember(FakeObjects.TestUser(), FakeObjects.TestRoles());

            testMember.AddRole(testRole);
            var addedRole = testMember.Roles.Where(x => x.Id == testRole.Id).FirstOrDefault();

            Assert.IsNotNull(addedRole);
            testMember.RemoveRole(testRole.Id);

            var removedRole = testMember.Roles.Where(x => x.Id == testRole.Id).FirstOrDefault();
            Assert.IsNull(removedRole);
        }

        #endregion
    }
}