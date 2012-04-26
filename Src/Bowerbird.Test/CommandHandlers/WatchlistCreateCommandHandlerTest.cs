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
    public class WatchlistCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;
            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void WatchlistCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();

            Watchlist newValue = null;

            var command = new WatchlistCreateCommand()
            {
                UserId = user.Id,
                Name = FakeValues.Name,
                JsonQuerystring = FakeValues.QuerystringJson
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);

                var commandHandler = new WatchlistCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<Watchlist>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Name, newValue.Name);
            Assert.AreEqual(command.JsonQuerystring, newValue.QuerystringJson);
        }

        #endregion
    }
}