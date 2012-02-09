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
    public class GroupAssociationCreateCommandHandlerTest
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

            GroupAssociation newValue = null;

            var command = new GroupAssociationCreateCommand()
            {
                UserId = user.Id,
                ChildGroupId = childGroup.Id,
                ParentGroupId = parentGroup.Id,
                CreatedDateTime = FakeValues.CreatedDateTime
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(childGroup);
                session.Store(parentGroup);
                session.SaveChanges();

                var commandHandler = new GroupAssociationCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<GroupAssociation>()
                    .Where(x => x.ChildGroupId == childGroup.Id && x.ParentGroupId == parentGroup.Id)
                    .FirstOrDefault();
            }

            Assert.IsNotNull(newValue);

            Assert.AreEqual(parentGroup.Id, newValue.ParentGroupId);
            Assert.AreEqual(childGroup.Id, newValue.ChildGroupId);
            Assert.AreEqual(user.Id, newValue.CreatedByUserId);
            Assert.AreEqual(command.CreatedDateTime, newValue.CreatedDateTime);
        }

        #endregion 
    }
}