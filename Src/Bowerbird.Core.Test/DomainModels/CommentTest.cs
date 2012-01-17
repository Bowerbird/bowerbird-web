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

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Test.Utils;

    #endregion

    public class CommentTest
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
        public void Comment_Constructor_Passing_Empty_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new Comment(null,FakeValues.CreatedDateTime, FakeValues.Message)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Comment_Constructor_Passing_Empty_Message_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new Comment(null, FakeValues.CreatedDateTime, string.Empty)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Comment_Constructor_Sets_Property_Values()
        {
            var testUser = FakeObjects.TestUser();
            var testComment = new Comment(
                testUser, 
                DateTime.Now,
                FakeValues.Comment);

            Assert.AreEqual(testComment.User.Id, testUser.Id);
            Assert.AreEqual(testComment.User.FirstName, testUser.FirstName);
            Assert.AreEqual(testComment.User.LastName, testUser.LastName);
            Assert.AreEqual(testComment.User.Email, testUser.Email);
            Assert.AreEqual(testComment.EditedOn.Day, DateTime.Now.Day);
            Assert.AreEqual(testComment.CommentedOn.Day, DateTime.Now.Day);
            Assert.AreEqual(testComment.Message, FakeValues.Comment);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Comment_User_Is_OfType_DenormalisedUserReference()
        {
            Assert.IsInstanceOf<DenormalisedUserReference>(
                new Comment(
                    FakeObjects.TestUser(), 
                    FakeValues.CreatedDateTime,
                    FakeValues.Message)
                    .User);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Comment_SubmittedOn_Is_OfType_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(
                new Comment(
                    FakeObjects.TestUser(),
                    FakeValues.CreatedDateTime,
                    FakeValues.Message)
                    .CommentedOn);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Comment_EditedOnOn_Is_OfType_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(
                new Comment(
                    FakeObjects.TestUser(),
                    FakeValues.CreatedDateTime,
                    FakeValues.Message)
                    .EditedOn);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Comment_Message_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(
                new Comment(
                    FakeObjects.TestUser(),
                    FakeValues.CreatedDateTime,
                    FakeValues.Message)
                    .Message);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}