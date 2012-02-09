/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class OrganisationDeleteCommandHandlerTest
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
        public void OrganisationDeleteCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();
            var organisation = FakeObjects.TestOrganisationWithId();

            Organisation deleted = null;

            var command = new OrganisationDeleteCommand()
            {
                Id = organisation.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(organisation);

                session.SaveChanges();

                var commandHandler = new OrganisationDeleteCommandHandler(session);
                commandHandler.Handle(command);
                session.SaveChanges();

                deleted = session.Load<Organisation>(organisation.Id);
            }

            Assert.IsNull(deleted);
        }

        #endregion
    }
}
