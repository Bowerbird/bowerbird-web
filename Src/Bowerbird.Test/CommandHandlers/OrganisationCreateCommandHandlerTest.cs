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

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class OrganisationCreateCommandHandlerTest
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
        public void OrganisationCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();

            Organisation newValue = null;

            var command = new OrganisationCreateCommand()
            {
                Description = FakeValues.Description,
                Name = FakeValues.Name,
                UserId = user.Id,
                Website = FakeValues.Website
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.SaveChanges();

                var commandHandler = new OrganisationCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<Organisation>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Website, newValue.Website);
            Assert.AreEqual(command.Description, newValue.Description);
            Assert.AreEqual(command.Name, newValue.Name);
        }

        #endregion
    }
}
