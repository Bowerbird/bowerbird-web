/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class ObservationNoteCreateCommandHandlerTest
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
        public void ObservationNoteCreateCommandHandler_Creates_ObservationNote()
        {
            var user = FakeObjects.TestUserWithId();
            var observation = FakeObjects.TestObservationWithId();

            ObservationNote newValue = null;

            var command = new ObservationNoteCreateCommand()
            {
                UserId = user.Id,
                CommonName = FakeValues.CommonName,
                Descriptions = new Dictionary<string, string>{{FakeValues.Description, FakeValues.Description}},
                Notes = FakeValues.Notes,
                ObservationId = observation.Id,
                References = new Dictionary<string, string> { { FakeValues.Description, FakeValues.Description } },
                ScientificName = FakeValues.ScientificName,
                SubmittedOn = FakeValues.CreatedDateTime,
                Tags = FakeValues.Tags,
                Taxonomy = FakeValues.Taxonomy
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(observation);

                var commandHandler = new ObservationNoteCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<ObservationNote>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.CommonName, newValue.CommonName);
            Assert.AreEqual(command.Descriptions, newValue.Descriptions);
            Assert.AreEqual(command.Notes, newValue.Notes);
            Assert.AreEqual(command.References, newValue.References);
            Assert.AreEqual(command.ScientificName, newValue.ScientificName);
            Assert.AreEqual(command.SubmittedOn, newValue.SubmittedOn);
            Assert.AreEqual(command.Tags, newValue.Tags);
            Assert.AreEqual(command.Taxonomy, newValue.Taxonomy);
            Assert.AreEqual(observation.DenormalisedObservationReference(), newValue.Observation);
        }

        #endregion
    }
}