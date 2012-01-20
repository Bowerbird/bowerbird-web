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
    public class ObservationCreateCommandHandlerTest
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
        public void ObservationCreateCommandHandler_Creates_Observation()
        {
            var user = FakeObjects.TestUserWithId();
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId();

            Observation newValue = null;

            var command = new ObservationCreateCommand()
            {
                UserId = user.Id,
                Address = FakeValues.Address,
                IsIdentificationRequired = FakeValues.IsTrue,
                Latitude = FakeValues.Latitude,
                Longitude = FakeValues.Longitude,
                MediaResources = new List<string>(){imageMediaResource.Id},
                ObservationCategory = FakeValues.Category,
                ObservedOn = FakeValues.CreatedDateTime,
                Title = FakeValues.Title
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(imageMediaResource);

                var commandHandler = new ObservationCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<Observation>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Address, newValue.Address);
            Assert.AreEqual(command.IsIdentificationRequired, newValue.IsIdentificationRequired);
            Assert.AreEqual(command.Latitude, newValue.Latitude);
            Assert.AreEqual(command.Longitude, newValue.Longitude);
            Assert.AreEqual(command.ObservationCategory, newValue.ObservationCategory);
            Assert.AreEqual(command.ObservedOn, newValue.ObservedOn);
            Assert.AreEqual(command.Title, newValue.Title);
            Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
            Assert.IsTrue(newValue.MediaResources.Count == 1);
            Assert.AreEqual(imageMediaResource, newValue.MediaResources[0]);
        }

        #endregion
    }
}