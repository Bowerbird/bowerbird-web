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
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class OrganisationUpdateCommandHandlerTest
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
        public void OrganisationUpdateCommandHandler_Updates_Organisation()
        {
            var user = FakeObjects.TestUserWithId();
            var organisation = FakeObjects.TestOrganisationWithId();

            Organisation newValue = null;

            var command = new OrganisationUpdateCommand()
            {
                Description = FakeValues.Description.PrependWith("new"),
                Id = organisation.Id,
                Name = FakeValues.Name.PrependWith("new"),
                UserId = user.Id,
                Website = FakeValues.Website.PrependWith("new")
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(organisation);
                session.SaveChanges();

                var commandHandler = new OrganisationUpdateCommandHandler(session);
                commandHandler.Handle(command);
                session.SaveChanges();

                newValue = session.Load<Organisation>(organisation.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Description, newValue.Description);
            Assert.AreEqual(command.Name, newValue.Name);
            Assert.AreEqual(command.Website, newValue.Website);
        }

        #endregion
    }
}