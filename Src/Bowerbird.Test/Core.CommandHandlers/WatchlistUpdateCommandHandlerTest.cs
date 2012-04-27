/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Core.CommandHandlers
{
    [TestFixture]
    public class WatchlistUpdateCommandHandlerTest
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
        public void WatchlistUpdateCommandHandlerTest_Handle()
        {
            var originalValue = FakeObjects.TestWatchlistWithId();
            var user = FakeObjects.TestUserWithId();
            Watchlist newValue;

            var command = new WatchlistUpdateCommand()
            {
                Id = originalValue.Id,
                Name = FakeValues.Name.PrependWith("new"),
                JsonQuerystring = FakeValues.QuerystringJson.PrependWith("new"),
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(originalValue);

                var commandHandler = new WatchlistUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<Watchlist>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Name, newValue.Name);
            Assert.AreEqual(command.JsonQuerystring, newValue.QuerystringJson);
        }

        #endregion
    }
}