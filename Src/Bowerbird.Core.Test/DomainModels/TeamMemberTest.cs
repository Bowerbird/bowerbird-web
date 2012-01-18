/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Members;

namespace Bowerbird.Core.Test.DomainModels
{
    #region Namespaces

    using System.Linq;

    using NUnit.Framework;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Core.DesignByContract;

    #endregion

    public class TeamMemberTest
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
        public void TeamMember_Constructor_Sets_Property_Values()
        {
            var testUser = FakeObjects.TestUser();
            var testRoles = FakeObjects.TestRoles();
            var testTeam = FakeObjects.TestTeam();

            var testMember = new TeamMember(testUser, testTeam, testUser, testRoles);

            Assert.AreEqual(testMember.Roles.Select(x => x.Id).ToList(), testRoles.Select(x => x.Id).ToList());
            Assert.AreEqual(testMember.Roles.Select(x => x.Name).ToList(), testRoles.Select(x => x.Name).ToList());
            Assert.AreEqual(testMember.User.FirstName, testUser.FirstName);
            Assert.AreEqual(testMember.User.LastName, testUser.LastName);
            Assert.AreEqual(testMember.User.Id, testUser.Id);
            Assert.AreEqual(testMember.Team.Id, testTeam.Id);
            Assert.AreEqual(testMember.Team.Name, testTeam.Name);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}