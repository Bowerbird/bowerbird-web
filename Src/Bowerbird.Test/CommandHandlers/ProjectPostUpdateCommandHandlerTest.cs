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
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class ProjectPostUpdateCommandHandlerTest
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
        public void ProjectPostUpdateCommandHandler_Updates_ProjectPost()
        {
            var originalValue = FakeObjects.TestProjectPostWithId();
            var project = FakeObjects.TestProjectWithId();
            var user = FakeObjects.TestUserWithId("abcabc");
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId(FakeValues.KeyString.AppendWith("abc"));

            ProjectPost newValue;

            var command = new ProjectPostUpdateCommand()
            {
                Id = originalValue.Id,
                MediaResources = new List<string>(){imageMediaResource.Id},
                Message = FakeValues.Message.PrependWith("new"),
                Subject = FakeValues.Subject.PrependWith("new"),
                Timestamp = FakeValues.ModifiedDateTime,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);
                session.Store(project);
                session.Store(imageMediaResource);
                session.Store(user);

                var commandHandler = new ProjectPostUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<ProjectPost>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Message, newValue.Message);
            Assert.AreEqual(command.Subject, newValue.Subject);
            //Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
            //Assert.IsTrue(newValue.MediaResources.Count == 1);
            //Assert.AreEqual(imageMediaResource, newValue.MediaResources[0]);
            Assert.AreEqual(command.Subject, newValue.Subject);
        }

        #endregion
    }
}