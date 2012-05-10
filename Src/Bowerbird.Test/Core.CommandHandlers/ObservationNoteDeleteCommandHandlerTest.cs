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

namespace Bowerbird.Test.Core.CommandHandlers
{
    [TestFixture]
    public class ObservationNoteDeleteCommandHandlerTest
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
        [Category(TestCategory.Unit)]
        public void ObservationNoteDeleteCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();
            var observationNote = FakeObjects.TestObservationNoteWithId();

            ObservationNote deletedTeam = null;

            var command = new DeleteCommand()
            {
                Id = observationNote.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(observation);
                session.Store(observationNote);

                var commandHandler = new ObservationNoteDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeam = session.Load<ObservationNote>(observationNote.Id);
            }

            Assert.IsNull(deletedTeam);
        }

        #endregion
    }
}