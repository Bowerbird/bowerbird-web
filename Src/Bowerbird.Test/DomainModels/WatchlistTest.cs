/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class WatchlistTest
    {
        #region Test Infrastructure

        const string additionalString = "_";

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Watchlist_Constructor()
        {
            var testWatchlist = new Watchlist(
                FakeObjects.TestUserWithId(), 
                FakeValues.Name,
                FakeValues.QuerystringJson);

            Assert.AreEqual(testWatchlist.Name, FakeValues.Name);
            Assert.AreEqual(testWatchlist.QuerystringJson, FakeValues.QuerystringJson);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Watchlist_UpdateDetails()
        {
            var testWatchlist = new Watchlist(FakeObjects.TestUser(), FakeValues.Name, FakeValues.QuerystringJson);

            testWatchlist.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Name.AppendWith(additionalString),
                FakeValues.QuerystringJson.AppendWith(additionalString));

            Assert.AreEqual(testWatchlist.Name, FakeValues.Name.AppendWith(additionalString));
            Assert.AreEqual(testWatchlist.QuerystringJson, FakeValues.QuerystringJson.AppendWith(additionalString));
        }

        #endregion
    }
}