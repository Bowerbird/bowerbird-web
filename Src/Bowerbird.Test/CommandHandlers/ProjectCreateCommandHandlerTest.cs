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
    public class ProjectCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;             
            DocumentStoreHelper.KillRaven();
       }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var team = FakeObjects.TestTeamWithId();

            Project newValue = null;

            var command = new ProjectCreateCommand()
            {
                UserId = user.Id,
                Description = FakeValues.Description,
                Name = FakeValues.Name,
                TeamId = team.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(team);
                session.SaveChanges();

                var commandHandler = new ProjectCreateCommandHandler(session);
                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<Project>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Name, newValue.Name);
            Assert.AreEqual(command.Description, newValue.Description);
            //Assert.AreEqual(team.DenormalisedNamedDomainModelReference<Team>(), newValue.Team);
        }

        #endregion 
    }
}