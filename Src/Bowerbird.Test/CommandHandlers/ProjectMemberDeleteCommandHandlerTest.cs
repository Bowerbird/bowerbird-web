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
    public class ProjectMemberDeleteCommandHandlerTest
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
        [Category(TestCategory.Integration)]
        public void ProjectMemberDeleteCommandHandler_Deletes_ProjectMember()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var projectMember = FakeObjects.TestProjectMemberWithId();

            ProjectMember deletedTeam = null;

            var command = new ProjectMemberDeleteCommand()
            {
                ProjectId = project.Id,
                UserId = user.Id,
                DeletedByUserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(projectMember);

                session.SaveChanges();

                var commandHandler = new ProjectMemberDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeam = session.Load<ProjectMember>(projectMember.Id);
            }

            Assert.IsNull(deletedTeam);
        }

        #endregion 
    }
}