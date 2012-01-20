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
using Bowerbird.Core.DomainModels.Comments;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;
using Bowerbird.Core.Commands;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class ObservationCommentCreateCommandHandlerTest
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
        public void ObservationCommentCreateCommandHandler_Creates_ObservationComment()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            ObservationComment newValue = null;

            var command = new ObservationCommentCreateCommand()
            {
                Comment = FakeValues.Comment,
                CommentedOn = FakeValues.CreatedDateTime,
                ObservationId = observation.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(observation);

                var commandHandler = new ObservationCommentCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<ObservationComment>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Comment, newValue.Message);
            Assert.AreEqual(command.CommentedOn, newValue.CommentedOn);
            Assert.AreEqual(observation.DenormalisedObservationReference(), newValue.Observation);
            Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
        }

        #endregion
    }
}