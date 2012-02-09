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
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class TeamCreateCommandHandlerTest
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
        public void TeamCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var organisation = FakeObjects.TestOrganisationWithId();

            Team newValue = null;

            var command = new TeamCreateCommand()
            {
                UserId = user.Id,
                Description = FakeValues.Description,
                Name = FakeValues.Name,
                Website = FakeValues.Website
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(organisation);

                var commandHandler = new TeamCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<Team>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Description , newValue.Description);
            Assert.AreEqual(command.Name, newValue.Name);
            Assert.AreEqual(command.Website, newValue.Website);
        }

        #endregion
    }
}