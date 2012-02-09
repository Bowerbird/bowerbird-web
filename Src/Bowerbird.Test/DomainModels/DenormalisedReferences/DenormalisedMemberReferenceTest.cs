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
using NUnit.Framework;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Test.Utils;
using Bowerbird.Core.DomainModels.Members;

namespace Bowerbird.Test.DomainModels.DenormalisedReferences
{
    [TestFixture]
    public class DenormalisedMemberReferenceTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static DenormalisedMemberReference TestDenormalise(Member member)
        {
            DenormalisedMemberReference denormalisedMemberReference = member;

            return denormalisedMemberReference;
        }

        public static GlobalMember TestGlobalMember()
        {
            var member = new GlobalMember(
                FakeObjects.TestUser(), 
                FakeObjects.TestRoles());

            ((IAssignableId) member).SetIdTo("globalmember",(new Random(System.DateTime.Now.Millisecond)).Next().ToString());

            return member;
        }

        public static GroupMember TestGroupMember()
        {
            var member = new GroupMember(
                FakeObjects.TestUser(), 
                FakeObjects.TestTeam(), 
                FakeObjects.TestUser(), 
                FakeObjects.TestRoles());

            ((IAssignableId)member).SetIdTo("groupmember", (new Random(System.DateTime.Now.Millisecond)).Next().ToString());

            return member;
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedMemberReference_Implicit_Operator_Passing_GlobalMember()
        {
            var normalisedMember = TestGlobalMember();

            DenormalisedMemberReference denormalisedMember = TestDenormalise(normalisedMember);

            Assert.AreEqual(normalisedMember.Id, denormalisedMember.Id);
            Assert.IsTrue(denormalisedMember.Type.Equals("globalmember"));
            Assert.AreEqual(denormalisedMember.Roles.ToList(), denormalisedMember.Roles.ToList());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedMemberReference_Implicit_Operator_Passing_ProjectMember()
        {
            var normalisedMember = TestGroupMember();

            DenormalisedMemberReference denormalisedMember = TestDenormalise(normalisedMember);

            Assert.AreEqual(normalisedMember.Id, denormalisedMember.Id);
            Assert.IsTrue(denormalisedMember.Type.Equals("groupmember"));
            Assert.AreEqual(denormalisedMember.Roles.ToList(), denormalisedMember.Roles.ToList());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedUserReference_Implicit_Operator_Passing_Null()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => TestDenormalise(null)));
        }

        #endregion
    }
}