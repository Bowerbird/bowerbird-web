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
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class TeamMemberDeleteCommandHandlerTest
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
        public void TeamMemberDeleteCommandHandler_Deletes_TeamMember()
        {
            var user = FakeObjects.TestUserWithId();
            var team = FakeObjects.TestTeamWithId();
            var teamMember = FakeObjects.TestTeamMemberWitId();

            TeamMember deletedTeamMember = null;

            var command = new TeamMemberDeleteCommand()
            {
                TeamId = teamMember.Team.Id,
                UserId = user.Id,
                MemberId = teamMember.User.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(team);
                session.Store(teamMember);

                session.SaveChanges();

                var commandHandler = new TeamMemberDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeamMember = session.Query<TeamMember>().FirstOrDefault();
            }

            Assert.IsNull(deletedTeamMember);
        }

        #endregion
    }
}