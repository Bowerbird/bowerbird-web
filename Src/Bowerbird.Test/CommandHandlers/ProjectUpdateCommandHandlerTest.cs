/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    public class ProjectUpdateCommandHandlerTest
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
        public void ProjectUpdateCommandHandler_Handle()
        {
            var originalValue = FakeObjects.TestProjectWithId();
            var team = FakeObjects.TestTeamWithId();
            var user = FakeObjects.TestUserWithId();

            Project newValue;

            var command = new ProjectUpdateCommand()
            {
                Description = FakeValues.Description,
                Name = FakeValues.Name,
                UserId = user.Id,
                TeamId = team.Id,
                Id = originalValue.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);
                session.Store(team);
                session.Store(user);

                var commandHandler = new ProjectUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<Project>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Name, newValue.Name);
            Assert.AreEqual(command.Description, newValue.Description);
            //Assert.AreEqual(team.DenormalisedNamedDomainModelReference<Team>(), newValue.Team);
        }

        #endregion 
    }
}