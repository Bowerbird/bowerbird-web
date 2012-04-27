/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Queries;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Core.Queries
{
    [TestFixture]
    public class UsersGroupsQueryTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            _documentStore = null;

            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Test Constructors

        [Test]
        public void UsersGroupsQuery_Constructor_Passed_Null_DocumentSession()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UsersGroupsQuery(null)));
        }

        #endregion

        #region Test Methods

        #endregion
    }
}