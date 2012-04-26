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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class ObservationGroupTest
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
        public void ObservationGroup_Constructor()
        {
            var user = FakeObjects.TestUserWithId();
            var createdDateTime = DateTime.UtcNow;

            var observationGroup = new ObservationGroup(
                FakeObjects.TestProjectWithId(),
                user,
                createdDateTime
                );

            Assert.AreEqual(user.DenormalisedUserReference(), observationGroup.User);
            Assert.AreEqual(FakeObjects.TestProjectWithId().Id, observationGroup.GroupId);
            Assert.AreEqual(FakeObjects.TestProjectWithId().GetType().Name.ToLower(), observationGroup.GroupType);
            Assert.AreEqual(createdDateTime, observationGroup.CreatedDateTime);

        }

        #endregion

        #region Method tests

        #endregion
    }
}