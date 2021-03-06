﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
    public class GroupAssociationTest
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
        public void GroupAssociation_Constructor()
        {
            var groupAssociation = new GroupAssociation(
                FakeObjects.TestTeamWithId(),
                FakeObjects.TestProjectWithId(),
                FakeObjects.TestUserWithId(),
                FakeValues.CreatedDateTime
                );

            Assert.AreEqual(FakeObjects.TestTeamWithId().Id, groupAssociation.ParentGroup.Id);
            Assert.AreEqual(FakeObjects.TestProjectWithId().Id, groupAssociation.ChildGroup.Id);
            Assert.AreEqual(FakeValues.CreatedDateTime, groupAssociation.CreatedDateTime);
            Assert.AreEqual(FakeObjects.TestUserWithId().Id, groupAssociation.CreatedByUserId);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}