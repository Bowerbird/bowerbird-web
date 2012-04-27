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
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
    public class CommentsComponentTest
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
        public void CommentsComponent_Constructor()
        {
            var commentsComponent = new CommentsComponent();

            Assert.IsNotNull(commentsComponent.Comments);
        }

        #endregion

        #region Methods tests

        [Test]
        [Category(TestCategory.Unit)]
        public void CommentsComponent_AddComment()
        {
            var commentsComponent = new CommentsComponent();
            var user = FakeObjects.TestUserWithId();
            var createdDateTime = DateTime.UtcNow;

            commentsComponent.AddComment(
                FakeValues.Message,
                user,
                createdDateTime
                );

            Assert.IsTrue(commentsComponent.Comments.ToList().Count == 1);
            Assert.AreEqual(commentsComponent.Comments.ToList()[0].Message, FakeValues.Message);
            Assert.AreEqual(commentsComponent.Comments.ToList()[0].User, user.DenormalisedUserReference());
            Assert.AreEqual(commentsComponent.Comments.ToList()[0].Id, "1");
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void CommentsComponent_RemoveComment()
        {
            var commentsComponent = new CommentsComponent();
            var user = FakeObjects.TestUserWithId();
            var createdDateTime = DateTime.UtcNow;

            commentsComponent.AddComment(
                FakeValues.Message,
                user,
                createdDateTime
                );

            Assert.IsTrue(commentsComponent.Comments.ToList().Count == 1);

            commentsComponent.RemoveComment(commentsComponent.Comments.ToList()[0].Id);

            Assert.IsTrue(commentsComponent.Comments.ToList().Count == 0);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void CommentsComponent_UpdateComment()
        {
            var commentsComponent = new CommentsComponent();
            var user = FakeObjects.TestUserWithId();
            var createdDateTime = DateTime.UtcNow.AddDays(-1);
            var modifiedDateTime = DateTime.UtcNow;

            commentsComponent.AddComment(
                FakeValues.Message,
                user,
                createdDateTime
                );

            commentsComponent.UpdateComment(
                commentsComponent.Comments.ToList()[0].Id,
                FakeValues.Message.AppendWith("new"),
                user,
                modifiedDateTime
                );

            Assert.AreEqual(commentsComponent.Comments.ToList()[0].Message, FakeValues.Message.AppendWith("new"));
            Assert.AreEqual(commentsComponent.Comments.ToList()[0].EditedOn, modifiedDateTime);
        }

        #endregion
    }
}