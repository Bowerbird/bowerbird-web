/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using NUnit.Framework;

using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class TeamTest
    {
        #region Test Infrastructure

        const string additionalString = "_";

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static Team TestTeam()
        {
            return new Team(
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
        public void Team_Constructor()
        {
            var testTeam = new Team(FakeObjects.TestUser(), FakeValues.Name, FakeValues.Description, FakeValues.Website, null);

            Assert.AreEqual(testTeam.Name, FakeValues.Name);
            Assert.AreEqual(testTeam.Description, FakeValues.Description);
            Assert.AreEqual(testTeam.Website, FakeValues.Website);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Team_UpdateDetails()
        {
            var testTeam = new Team(FakeObjects.TestUser(), FakeValues.Name, FakeValues.Description, FakeValues.Website, null);

            testTeam.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Name.AppendWith(additionalString),
                FakeValues.Description.AppendWith(additionalString),
                FakeValues.Website.AppendWith(additionalString),
                null);

            Assert.AreEqual(testTeam.Name, FakeValues.Name.AppendWith(additionalString));
            Assert.AreEqual(testTeam.Description, FakeValues.Description.AppendWith(additionalString));
            Assert.AreEqual(testTeam.Website, FakeValues.Website.AppendWith(additionalString));
        }

        #endregion
    }
}