/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class ProjectTest
    {
        #region Test Infrastructure

        const string additionalString = "_";

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static Project TestProject()
        {
            return new Project(
                FakeObjects.TestUser(), 
                FakeValues.Name, 
                FakeValues.Description,
                FakeValues.Website, 
                null
                );
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Constructor()
        {
            var testProject = new Project(
                FakeObjects.TestUser(), 
                FakeValues.Name, 
                FakeValues.Description,
                FakeValues.Website,
                null);

            Assert.AreEqual(testProject.Name, FakeValues.Name);
            Assert.AreEqual(testProject.Description, FakeValues.Description);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_UpdateDetails()
        {
            var testProject = TestProject();

            testProject.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Name.AppendWith(additionalString),
                FakeValues.Description.AppendWith(additionalString),
                FakeValues.Website.AppendWith(additionalString),
                null
                );

            Assert.AreEqual(testProject.Name, FakeValues.Name.AppendWith(additionalString));
            Assert.AreEqual(testProject.Description, FakeValues.Description.AppendWith(additionalString));
        }

        #endregion
    }
}