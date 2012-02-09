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
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    public class GroupMemberDeleteCommandHandlerTest
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
        [Category(TestCategory.Integration)]
        public void GroupMemberDeleteCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var groupMember = FakeObjects.TestGroupMemberWithId();

            GroupMember deletedTeam = null;

            var command = new GroupMemberDeleteCommand()
            {
                GroupId = project.Id,
                UserId = user.Id,
                DeletedByUserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(groupMember);

                session.SaveChanges();

                var commandHandler = new GroupMemberDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeam = session.Load<GroupMember>(groupMember.Id);
            }

            Assert.IsNull(deletedTeam);
        }

        #endregion 
    }
}