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
using Bowerbird.Core.Extensions;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class ObservationUpdateCommandHandlerTest
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
        [Category(TestCategory.Persistance)]
        public void ObservationUpdateCommandHandlerTest_Handle()
        {
            var originalValue = FakeObjects.TestObservationWithId();
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId("abcabc");
            var user = FakeObjects.TestUserWithId();

            Observation newValue;

            var command = new ObservationUpdateCommand()
            {
                Id = originalValue.Id,
                UserId = user.Id,
                Address = FakeValues.Address.PrependWith("new"),
                IsIdentificationRequired = !originalValue.IsIdentificationRequired,
                Latitude = FakeValues.Latitude.PrependWith("new"),
                Longitude = FakeValues.Longitude.PrependWith("new"),
                ObservationCategory = FakeValues.Category.PrependWith("new"),
                Title = FakeValues.Title.PrependWith("new"),
                ObservedOn = FakeValues.ModifiedDateTime,
                ObservationMediaItems = new Dictionary<string, string>(){ {imageMediaResource.Id, FakeValues.Description }}
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);
                session.Store(imageMediaResource);
                session.Store(user);
                session.SaveChanges();

                var commandHandler = new ObservationUpdateCommandHandler(session);
                commandHandler.Handle(command);
                session.SaveChanges();

                newValue = session.Load<Observation>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Address, newValue.Address);
            Assert.AreEqual(command.IsIdentificationRequired, newValue.IsIdentificationRequired);
            Assert.AreEqual(command.Latitude, newValue.Latitude);
            Assert.AreEqual(command.Longitude, newValue.Longitude);
            Assert.AreEqual(command.ObservationCategory, newValue.ObservationCategory);
            Assert.AreEqual(command.Title, newValue.Title);
            Assert.AreEqual(command.ObservedOn, newValue.ObservedOn);
            Assert.IsTrue(newValue.ObservationMedia.ToList().Count == 1);
            Assert.AreEqual(imageMediaResource, newValue.ObservationMedia.ToList()[0].MediaResource);
            Assert.AreEqual(FakeValues.Description, newValue.ObservationMedia.ToList()[0].Description);
        }

        #endregion 
    }
}