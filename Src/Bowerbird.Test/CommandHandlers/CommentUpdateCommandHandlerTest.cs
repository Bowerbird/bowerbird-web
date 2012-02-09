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
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class CommentUpdateCommandHandlerTest
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
        public void CommentUpdateCommandHandler_Handle()
        {
            var originalValue = FakeObjects.TestCommentWithId();
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            Comment newValue;

            var command = new CommentUpdateCommand()
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

                var commandHandler = new CommentUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<Comment>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Comment, newValue.Message);
        }

        #endregion
    }
}