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

    using System.Linq;

    using NUnit.Framework;

    using Bowerbird.Core.Entities;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.Entities.DenormalisedReferences;
    using Bowerbird.Core.DesignByContract;

    #endregion

    public class ProjectMemberTest
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
        public void ProjectMember_Constructor_Passing_Null_Project_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProjectMember(FakeObjects.TestUser(), null, FakeObjects.TestUser(), FakeObjects.TestRoles())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Constructor_Passing_Null_CreatedByUser_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProjectMember(null, FakeObjects.TestProject(), FakeObjects.TestUser(), FakeObjects.TestRoles())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Constructor_Sets_Property_Values()
        {
            var testUser = FakeObjects.TestUser();
            var testRoles = FakeObjects.TestRoles();
            var testProject = FakeObjects.TestProject();
            
            var testMember = new ProjectMember(testUser, testProject, testUser, testRoles);

            Assert.AreEqual(testMember.Roles.Select(x => x.Id).ToList(), testRoles.Select(x => x.Id).ToList());
            Assert.AreEqual(testMember.Roles.Select(x => x.Name).ToList(), testRoles.Select(x => x.Name).ToList());
            Assert.AreEqual(testMember.User.Email, testUser.Email);
            Assert.AreEqual(testMember.User.FirstName, testUser.FirstName);
            Assert.AreEqual(testMember.User.LastName, testUser.LastName);
            Assert.AreEqual(testMember.User.Id, testUser.Id);
            Assert.AreEqual(testMember.Project.Id, testProject.Id);
            Assert.AreEqual(testMember.Project.Name, testProject.Name);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Project_Is_OfType_DenormalisedNamedEntityReference_Project()
        {
            var testMember = new ProjectMember(FakeObjects.TestUser(), FakeObjects.TestProject(), FakeObjects.TestUser(), FakeObjects.TestRoles());

            Assert.IsInstanceOf<DenormalisedNamedEntityReference<Project>>(testMember.Project);
        }

        #endregion

        #region Method tests

        #endregion
    }
}