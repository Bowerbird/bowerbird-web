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
        public void Comment_Constructor()
        {
            var comment = new Comment(
                FakeObjects.TestUserWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Comment
                );

            Assert.AreEqual(FakeObjects.TestUserWithId().DenormalisedUserReference(), comment.User);
            Assert.AreEqual(FakeValues.CreatedDateTime, comment.CommentedOn);
            Assert.AreEqual(FakeValues.Comment, comment.Message);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Comment_UpdateDetails()
        {
            var comment = new Comment(
                FakeObjects.TestUserWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Comment
                );

            comment.UpdateDetails(
                FakeObjects.TestUserWithId(),
                FakeValues.ModifiedDateTime,
                FakeValues.Comment.PrependWith("new")
                );

            Assert.AreEqual(comment.Message, FakeValues.Comment.PrependWith("new"));
        }

        #endregion 
    }
}