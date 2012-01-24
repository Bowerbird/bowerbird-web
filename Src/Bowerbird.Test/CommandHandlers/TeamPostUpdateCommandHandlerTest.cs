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
    public class TeamPostUpdateCommandHandlerTest
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
        public void TeamPostUpdateCommandHandler_Updates_TeamPost()
        {
            var originalValue = FakeObjects.TestTeamPostWithId();
            TeamPost newValue;

            var newImageMediaResource = FakeObjects.TestImageMediaResourceWithId("123123");
            var user = FakeObjects.TestUserWithId();

            var command = new TeamPostUpdateCommand()
            {
                Id = originalValue.Id,
                MediaResources = new List<string>(){newImageMediaResource.Id},
                Message = FakeValues.Message.PrependWith("new"),
                Subject = FakeValues.Subject.PrependWith("new"),
                Timestamp = FakeValues.ModifiedDateTime,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);
                session.Store(newImageMediaResource);
                session.Store(user);

                var commandHandler = new TeamPostUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<TeamPost>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Message, newValue.Message);
            Assert.AreEqual(command.Subject, newValue.Subject);
            //Assert.IsTrue(newValue.MediaResources.Count == 1);
            //Assert.AreEqual(newImageMediaResource, newValue.MediaResources[0]);
            Assert.AreEqual(command.UserId, newValue.User.Id);
        }

        #endregion
    }
}