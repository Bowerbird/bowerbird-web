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
    public class PostCreateCommandHandlerTest
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
        public void PostCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId();

            Post newValue = null;

            var command = new PostCreateCommand()
            {
                UserId = user.Id,
                GroupId = project.Id,
                MediaResources = new List<string>(){imageMediaResource.Id},
                Message = FakeValues.Message,
                Subject = FakeValues.Subject,
                Timestamp = FakeValues.CreatedDateTime
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(imageMediaResource);

                var commandHandler = new PostCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<Post>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            //Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
            //Assert.AreEqual(project.DenormalisedNamedDomainModelReference(), newValue.Project);
            //Assert.IsTrue(newValue.MediaResources.Count == 1);
            //Assert.AreEqual(imageMediaResource, newValue.MediaResources[0]);
            Assert.AreEqual(command.Message, newValue.Message);
            Assert.AreEqual(command.Subject, newValue.Subject);
            Assert.AreEqual(command.Timestamp, newValue.CreatedOn);
        }

        #endregion 
    }
}