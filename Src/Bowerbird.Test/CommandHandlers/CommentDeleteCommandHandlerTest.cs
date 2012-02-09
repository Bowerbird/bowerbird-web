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
    public class CommentDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.InMemoryDocumentStore();
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
        public void CommentDeleteCommandHandler_Handle()
        {
            var observation = FakeObjects.TestObservationWithId();
            var observationComment = FakeObjects.TestCommentWithId();
            var user = FakeObjects.TestUserWithId();

            Comment deletedTeam = null;

            var command = new CommentDeleteCommand()
            {
                Id = observationComment.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(observation);
                session.Store(observationComment);
                session.Store(user);

                var commandHandler = new CommentDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeam = session.Load<Comment>(observationComment.Id);
            }

            Assert.IsNull(deletedTeam);
        }

        #endregion
    }
}