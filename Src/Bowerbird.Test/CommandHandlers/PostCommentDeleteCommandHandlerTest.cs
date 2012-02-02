/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels.Comments;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class PostCommentDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void PostCommentDeleteCommandHandler_Deletes_PostComment()
        {
            var post = FakeObjects.TestPostWithId();
            var user = FakeObjects.TestUserWithId();
            var postComment = FakeObjects.TestPostCommentWithId();

            PostComment deletedTeam = null;

            var command = new PostCommentDeleteCommand()
            {
                Id = postComment.Id,
                UserId = user.Id//must be same as teamPost.User.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(post);
                session.Store(postComment);

                var commandHandler = new PostCommentDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeam = session.Load<PostComment>(postComment.Id);
            }

            Assert.IsNull(deletedTeam);
        }

        #endregion
    }
}