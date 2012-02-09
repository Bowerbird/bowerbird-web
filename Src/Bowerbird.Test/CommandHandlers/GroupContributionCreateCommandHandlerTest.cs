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
    public class GroupContributionCreateCommandHandlerTest
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
        public void GroupContributionCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var contribution = FakeObjects.TestObservationWithId();
            var group = FakeObjects.TestProjectWithId();

            GroupContribution newValue = null;

            var command = new GroupContributionCreateCommand()
            {
                ContributionId = contribution.Id,
                CreatedDateTime = FakeValues.CreatedDateTime,
                GroupId = group.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(contribution);
                session.Store(group);
                session.SaveChanges();

                var commandHandler = new GroupContributionCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<GroupContribution>()
                    .Where(x => x.ContributionId == contribution.Id && x.GroupId == group.Id)
                    .FirstOrDefault();
            }

            Assert.IsNotNull(newValue);

            Assert.AreEqual(contribution.Id, newValue.ContributionId);
            Assert.AreEqual(group.Id, newValue.GroupId);
            Assert.AreEqual(user.DenormalisedUserReference(), newValue.User);
        }

        #endregion 
    }
}