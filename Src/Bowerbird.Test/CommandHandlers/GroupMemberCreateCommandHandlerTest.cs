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
    public class GroupMemberCreateCommandHandlerTest
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
        public void GroupMemberCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var permissions = FakeObjects.TestPermissions();
            var roles = FakeObjects.TestRoles();

            GroupMember newValue = null;

            var command = new GroupMemberCreateCommand()
            {
                UserId = user.Id,
                CreatedByUserId = user.Id,
                GroupId = project.Id,
                Roles = roles.Select(x => x.Name).ToList()
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                foreach (var permission in permissions)session.Store(permission);
                foreach (var role in roles)session.Store(role);

                session.SaveChanges();

                var commandHandler = new GroupMemberCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<GroupMember>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(project.Id, newValue.Group.Id);
            Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
        }

        #endregion 
    }
}