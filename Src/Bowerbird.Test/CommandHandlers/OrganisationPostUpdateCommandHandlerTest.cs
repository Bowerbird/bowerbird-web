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
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class OrganisationPostUpdateCommandHandlerTest
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
        public void OrganisationPostUpdateCommandHandler_Updates_OrganisationPost()
        {
            var organisation = FakeObjects.TestOrganisationWithId();
            var user = FakeObjects.TestUserWithId();
            var organisationPost = FakeObjects.TestOrganisationPostWithId();
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId("abcabc");

            OrganisationPost newValue = null;

            var command = new OrganisationPostUpdateCommand()
            {
                MediaResources = new List<string>() { imageMediaResource.Id },
                Message = FakeValues.Message.PrependWith("new"),
                Subject = FakeValues.Subject.PrependWith("new"),
                UserId = user.Id,
                Id = organisationPost.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(organisation);
                session.Store(imageMediaResource);
                session.Store(organisationPost);
                session.SaveChanges();

                var commandHandler = new OrganisationPostUpdateCommandHandler(session);
                commandHandler.Handle(command);
                session.SaveChanges();

                newValue = session.Load<OrganisationPost>(organisationPost.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Message, newValue.Message);
            Assert.AreEqual(command.Subject, newValue.Subject);
            Assert.IsTrue(newValue.MediaResources.Count == 1);
            Assert.AreEqual(imageMediaResource, newValue.MediaResources[0]);
        }

        #endregion
    }
}