/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels.Comments;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class PostCommentCreateCommandHandlerTest
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
        public void PostCommentCreateCommandHandler_Creates_ObservationComment()
        {
            var user = FakeObjects.TestUserWithId();
            var projectPost = FakeObjects.TestProjectPostWithId();

            PostComment newValue = null;

            var command = new PostCommentCreateCommand()
            {
                UserId = user.Id,
                Message = FakeValues.Message,
                PostedOn = FakeValues.CreatedDateTime,
                PostId = projectPost.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(projectPost);

                var commandHandler = new PostCommentCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<PostComment>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Message, newValue.Message);
            Assert.AreEqual(command.PostedOn, newValue.CommentedOn);
            Assert.AreEqual(projectPost, newValue.Post);
            //Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
        }

        #endregion
    }
}