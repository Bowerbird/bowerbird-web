/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.DomainModels
{
    #region Namespaces

    using System;

    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;

    #endregion

    public class ActivityTest
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
        public void Activity_Constructor_Sets_Property_Values()
        {
            var testUser = FakeObjects.TestUser();
            var testData = new { Test = "Test" };
            var testActivity = new Activity(FakeValues.ActivityType, testUser, testData);

            Assert.AreEqual(testUser, testActivity.User);
            Assert.AreEqual(testData, testActivity.Data);
            Assert.AreEqual(testActivity.Type, FakeValues.ActivityType);
            Assert.AreEqual(testActivity.OccurredOn.Day, DateTime.Now.Day);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}