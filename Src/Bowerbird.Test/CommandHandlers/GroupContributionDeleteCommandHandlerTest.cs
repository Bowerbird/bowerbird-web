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
    [TestFixture]
    public class GroupContributionDeleteCommandHandlerTest
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
        public void GroupContributionDeleteCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var contribution = FakeObjects.TestObservationWithId();
            var group = FakeObjects.TestProjectWithId();

            var groupContribution = new GroupContribution(
                group,
                contribution,
                user,
                FakeValues.CreatedDateTime);

            GroupContribution deletedValue;

            var command = new GroupContributionDeleteCommand() 
            { 
                ContributionId = contribution.Id, 
                GroupId = group.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(contribution);
                session.Store(group);
                session.Store(groupContribution);
                session.SaveChanges();

                Assert.IsNotNull(session.Query<GroupContribution>()
                    .Where(x => x.ContributionId == contribution.Id && x.GroupId == group.Id)
                    .FirstOrDefault());

                var commandHandler = new GroupContributionDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedValue = session.Query<GroupContribution>()
                    .Where(x => x.ContributionId == contribution.Id && x.GroupId == group.Id)
                    .FirstOrDefault();
            }

            Assert.IsNull(deletedValue);
        }

        #endregion 
    }
}