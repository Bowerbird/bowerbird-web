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
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class PostDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;             
            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void PostDeleteCommandHandler_Handle()
        {
            var post = FakeObjects.TestPostWithId();
            var user = FakeObjects.TestUserWithId();

            Post deletedPost = null;

            var command = new PostDeleteCommand()
            {
                Id = post.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(post);
                session.Store(user);

                var commandHandler = new PostDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedPost = session.Load<Post>(post.Id);
            }

            Assert.IsNull(deletedPost);
        }

        #endregion 
    }
}