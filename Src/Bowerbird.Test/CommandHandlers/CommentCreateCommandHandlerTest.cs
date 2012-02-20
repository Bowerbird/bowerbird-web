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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Indexes;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class CommentCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.ServerDocumentStore();
            //_store = DocumentStoreHelper.InMemoryDocumentStore();
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
        public void CommentCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            Observation savedObservation = null;
            Comment comment = null;

            var command = new CommentCreateCommand()
            {
                Comment = FakeValues.Comment,
                CommentedOn = FakeValues.CreatedDateTime,
                ContributionId = observation.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession(DocumentStoreHelper.TestDb))
            {
                session.Store(user);
                session.Store(observation);
                session.SaveChanges();

                var commandHandler = new CommentCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                savedObservation = session.Query<Observation>()
                    .Where(x => x.Id == observation.Id)
                    .FirstOrDefault();

                Assert.IsTrue(savedObservation.Comments.IsNotNullAndHasItems());

                comment = savedObservation.Comments.ToList()[0];
            }
    
            Assert.IsNotNull(comment);
            Assert.AreEqual(command.Comment, comment.Message);
            Assert.AreEqual(command.CommentedOn, comment.CommentedOn);
            Assert.AreEqual(command.UserId, comment.User.Id);
        }

        #endregion
    }
}