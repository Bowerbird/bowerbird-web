/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DomainModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;

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
        public void Activity_Constructor_Passing_Empty_Type_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Activity(string.Empty, FakeObjects.TestUser(), new object())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Activity_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Activity(FakeValues.ActivityType,null,new object())));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Activity_Constructor_Passing_Null_Object_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Activity(FakeValues.ActivityType, FakeObjects.TestUser(), null)));
        }

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

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Activity_OccurredOn_Is_OfType_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(new Activity(FakeValues.ActivityType, FakeObjects.TestUser(),new object()).OccurredOn);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Activity_Type_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(new Activity(FakeValues.ActivityType, FakeObjects.TestUser(), new object()).Type);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Activity_User_Is_OfType_User()
        {
            Assert.IsInstanceOf<User>(new Activity(FakeValues.ActivityType, FakeObjects.TestUser(), new object()).User);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Activity_Data_Is_OfType_Object()
        {
            Assert.IsInstanceOf<object>(new Activity(FakeValues.ActivityType, FakeObjects.TestUser(), new object()).Data);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}