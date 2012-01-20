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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;
using Bowerbird.Core.Commands;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class ProjectMemberCreateCommandHandlerTest
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
        public void ProjectMemberCreateCommandHandler_Creates_ProjectMember()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var roles = FakeObjects.TestRoles();

            ProjectMember newValue = null;

            var command = new ProjectMemberCreateCommand()
            {
                UserId = user.Id,
                CreatedByUserId = user.Id,
                ProjectId = project.Id,
                Roles = roles.Select(x => x.Name).ToList()
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(roles);

                var commandHandler = new ProjectMemberCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<ProjectMember>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(roles.Select(x => x.DenormalisedNamedDomainModelReference<Role>()).ToList(), newValue.Roles);
            Assert.AreEqual(project.DenormalisedNamedDomainModelReference(), newValue.Project);
            Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
        }

        #endregion 
    }
}