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
    public class ObservationCommentDeleteCommandHandlerTest
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
        public void ObservationCommentDeleteCommandHandler_Deletes_ObservationComment()
        {
            var observation = FakeObjects.TestObservationWithId();
            var observationComment = FakeObjects.TestObservationCommentWithId();
            var user = FakeObjects.TestUserWithId();

            ObservationComment deletedTeam = null;

            var command = new ObservationCommentDeleteCommand()
            {
                Id = observationComment.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(observation);
                session.Store(observationComment);
                session.Store(user);

                var commandHandler = new ObservationCommentDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeam = session.Load<ObservationComment>(observationComment.Id);
            }

            Assert.IsNull(deletedTeam);
        }

        #endregion
    }
}