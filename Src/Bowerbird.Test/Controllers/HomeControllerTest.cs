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
using System.Collections.Generic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers;
using Bowerbird.Web.ViewModels;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private HomeController _controller;

        [SetUp]
        public void TestInitialize()
        {
            //_documentStore = DocumentStoreHelper.ServerDocumentStore();
            _documentStore = DocumentStoreHelper.StartRaven();

            _mockCommandProcessor = new Mock<ICommandProcessor>();

            _mockUserContext = new Mock<IUserContext>();

            _controller = new HomeController(
                _mockCommandProcessor.Object,
                _mockUserContext.Object
                );
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        /// <summary>
        /// Create a user, team and project
        /// Add the user as a group member to the team and project
        /// 
        /// Query the Home Controller for the index passing a user's id, page number and page size
        /// 
        /// Check that the response contains:
        /// The user's profile
        /// A list of menu items containing the project
        /// A list of menu items containing the team
        /// A list of menu items containing the watchlist
        /// </summary>
        [Test]
        [Category(TestCategory.Unit)]
        public void HomeController_PrivateIndex()
        {

        }

        [Test]
        [Category(TestCategory.Unit)]
        public void HomeController_PublicIndex()
        { 
        
        }

        #endregion
    }
}