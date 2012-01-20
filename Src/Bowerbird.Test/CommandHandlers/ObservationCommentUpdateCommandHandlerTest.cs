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
    public class ObservationCommentUpdateCommandHandlerTest
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
        public void ObservationCommentUpdateCommandHandler_Updates_ObservationComment()
        {
            var originalValue = FakeObjects.TestObservationCommentWithId();
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            ObservationComment newValue;

            var command = new ObservationCommentUpdateCommand()
            {
                Id = originalValue.Id,
                Comment = FakeValues.Comment.PrependWith("new"),
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);
                session.Store(user);
                session.Store(observation);

                var commandHandler = new ObservationCommentUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<ObservationComment>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Comment, newValue.Message);
        }

        #endregion
    }
}