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
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Core.CommandHandlers
{
    [TestFixture]
    public class ObservationNoteUpdateCommandHandlerTest
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
        public void ObservationNoteUpdateCommandHandler_Handle()
        {
            var originalValue = FakeObjects.TestObservationNoteWithId();
            var observation = FakeObjects.TestObservationWithId();
            var user = FakeObjects.TestUserWithId();

            ObservationNote newValue;

            var command = new ObservationNoteUpdateCommand()
            {
                Id = originalValue.Id,
                UserId = user.Id,
                CommonName = FakeValues.CommonName.PrependWith("new"),
                ScientificName = FakeValues.ScientificName.PrependWith("new"),
                Notes = FakeValues.Notes.PrependWith("new"),
                Tags = FakeValues.Tags.PrependWith("new"),
                Taxonomy = FakeValues.Taxonomy.PrependWith("new"),
                SubmittedOn = FakeValues.ModifiedDateTime,
                References = new Dictionary<string, string>() { { FakeValues.Description.PrependWith("new"), FakeValues.Description.PrependWith("new") } },
                Descriptions = new Dictionary<string, string>() { { FakeValues.Description.PrependWith("new"), FakeValues.Description.PrependWith("new") } },
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);
                session.Store(observation);
                session.Store(user);

                var commandHandler = new ObservationNoteUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<ObservationNote>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.CommonName, newValue.CommonName);
            Assert.AreEqual(command.ScientificName, newValue.ScientificName);
            Assert.AreEqual(command.Tags, newValue.Tags);
            Assert.AreEqual(command.Taxonomy, newValue.Taxonomy);
            Assert.AreEqual(command.References, newValue.References);
            Assert.AreEqual(command.Descriptions, newValue.Descriptions);
        }

        #endregion
    }
}