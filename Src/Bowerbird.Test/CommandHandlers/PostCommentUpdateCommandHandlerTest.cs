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
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DomainModels.Comments;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class PostCommentUpdateCommandHandlerTest
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
        [Category(TestCategory.Persistance)]
        public void PostCommentUpdateCommandHandler_Updates_PostComment()
        {
            var originalValue = FakeObjects.TestPostCommentWithId();
            var user = FakeObjects.TestUserWithId();
            var post = FakeObjects.TestPostWithId();

            PostComment newValue;

            var command = new PostCommentUpdateCommand()
            {
                Id = originalValue.Id,
                Comment = FakeValues.Comment.PrependWith("new"),
                UserId = user.Id,
                UpdatedOn = FakeValues.ModifiedDateTime
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);
                session.Store(user);
                session.Store(post);

                var commandHandler = new PostCommentUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<PostComment>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Comment, newValue.Message);
        }

        #endregion
    }
}