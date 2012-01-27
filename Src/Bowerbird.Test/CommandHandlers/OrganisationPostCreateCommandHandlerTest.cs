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
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class OrganisationPostCreateCommandHandlerTest
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
        public void OrganisationPostCreateCommandHandler_Creates_ObservationPost()
        {
            var user = FakeObjects.TestUserWithId();
            var organisation = FakeObjects.TestOrganisationWithId();
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId();

            OrganisationPost newValue = null;

            var command = new OrganisationPostCreateCommand()
            {
                MediaResources = new List<string>() {imageMediaResource.Id},
                Message = FakeValues.Message,
                OrganisationId = organisation.Id,
                PostedOn = FakeValues.CreatedDateTime,
                Subject = FakeValues.Subject,
                UserId = user.Id
            };

            using(var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(organisation);
                session.Store(imageMediaResource);
                session.SaveChanges();

                var commandHandler = new OrganisationPostCreateCommandHandler(session);
                commandHandler.Handle(command);
                session.SaveChanges();

                newValue = session.Query<OrganisationPost>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Message, newValue.Message);
            Assert.AreEqual(command.PostedOn, newValue.PostedOn);
            Assert.AreEqual(command.Subject, newValue.Subject);
            Assert.AreEqual(organisation.DenormalisedNamedDomainModelReference<Organisation>(), newValue.Organisation);
            Assert.IsTrue(newValue.MediaResources.Count == 1);
            Assert.AreEqual(imageMediaResource, newValue.MediaResources[0]);
        }

        #endregion
    }
}