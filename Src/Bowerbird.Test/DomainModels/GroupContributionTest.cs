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
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class GroupContributionTest
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
        public void GroupContribution_Constructor()
        {
            var groupContribution = new GroupContribution(
                FakeObjects.TestProjectWithId(),
                FakeObjects.TestObservationWithId(),
                FakeObjects.TestUserWithId(),
                FakeValues.CreatedDateTime
                );

            Assert.AreEqual(FakeObjects.TestProjectWithId().Id, groupContribution.GroupId);
            Assert.AreEqual(FakeObjects.TestObservationWithId().Id, groupContribution.ContributionId);
            Assert.AreEqual(FakeObjects.TestUserWithId().DenormalisedUserReference(), groupContribution.User);
            Assert.AreEqual(FakeValues.CreatedDateTime, groupContribution.CreatedDateTime);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}