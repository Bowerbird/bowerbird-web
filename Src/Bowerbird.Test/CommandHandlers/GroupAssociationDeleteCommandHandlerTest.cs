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
using Raven.Client.Linq;

namespace Bowerbird.Test.CommandHandlers
{
    public class GroupAssociationDeleteCommandHandlerTest
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
        public void GroupAssociationCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var parentGroup = FakeObjects.TestTeamWithId();
            var childGroup = FakeObjects.TestProjectWithId();

            var groupAssociation = new GroupAssociation(
                parentGroup,
                childGroup,
                user,
                FakeValues.CreatedDateTime);

            GroupAssociation deletedValue;

            var command = new GroupAssociationDeleteCommand()
            {
                UserId = user.Id,
                ChildGroupId = childGroup.Id,
                ParentGroupId = parentGroup.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(childGroup);
                session.Store(parentGroup);
                session.Store(groupAssociation);
                
                session.SaveChanges();
                Assert.IsNotNull(
                    session
                    .Query<GroupAssociation>()
                    .Where(x => x.ChildGroupId == childGroup.Id && x.ParentGroupId == parentGroup.Id)
                    .FirstOrDefault()
                );

                var commandHandler = new GroupAssociationDeleteCommandHandler(session);
                commandHandler.Handle(command);
                session.SaveChanges();

                deletedValue = session.Query<GroupAssociation>()
                    .Where(x => x.ChildGroupId == childGroup.Id && x.ParentGroupId == parentGroup.Id)
                    .FirstOrDefault();
            }

            Assert.IsNull(deletedValue);
        }

        #endregion 
    }
}