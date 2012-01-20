/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class TeamUpdateCommandHandlerTest
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
        private void TeamUpdateCommandHandlerTest_Updates_Team()
        {
            var originalValue = FakeObjects.TestTeamWithId();
            Team newValue;

            var command = new TeamUpdateCommand()
            {
                Description = FakeValues.Description.PrependWith("new"),
                Id = originalValue.Id,
                Name = FakeValues.Name.PrependWith("new"),
                UserId = FakeValues.KeyString
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);

                var commandHandler = new TeamUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<Team>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Description, newValue.Description);
            Assert.AreEqual(command.Name, newValue.Name);
        }

        #endregion
    }
}