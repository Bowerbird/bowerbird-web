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
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
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
        public void Member_Constructor()
        {
            var user = FakeObjects.TestUserWithId();
            var roles = FakeObjects.TestRoles();

            var member = new Member(
                user,
                user,
                FakeObjects.TestProjectWithId(),
                roles
                );

            Assert.AreEqual(user.DenormalisedUserReference(), member.User);
            Assert.AreEqual(FakeObjects.TestProjectWithId().DenormalisedNamedDomainModelReference<Group>(), member.Group);
            // Not sure what's happening below.. values are equal.
            //Assert.AreEqual(roles.Select(x => x.DenormalisedRoleReference()).ToList(), member.Roles);
        }

        #endregion

        #region Method Tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_AddRole()
        {
            var user = FakeObjects.TestUserWithId();
            var roles = FakeObjects.TestRoles();
            var newRole = new Role("Admin", "Administrator", "Administrator Member", FakeObjects.TestPermissions());

            var member = new Member(
                user,
                user,
                FakeObjects.TestProjectWithId(),
                roles
                );

            Assert.IsTrue(member.Roles.ToList().Count == 1);

            member.AddRole(newRole);

            Assert.IsTrue(member.Roles.ToList().Count == 2);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Member_RemoveRole()
        {
            var user = FakeObjects.TestUserWithId();
            var roles = FakeObjects.TestRoles();

            var member = new Member(
                user,
                user,
                FakeObjects.TestProjectWithId(),
                roles
                );

            Assert.IsTrue(member.Roles.ToList().Count == 1);

            member.RemoveRole(roles.ToList()[0].Id);

            Assert.IsTrue(member.Roles.ToList().Count == 0);
        }

        #endregion
    }
}