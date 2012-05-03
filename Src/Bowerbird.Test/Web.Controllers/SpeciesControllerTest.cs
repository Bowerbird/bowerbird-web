/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Controllers;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Web.Controllers
{
    [TestFixture]
    public class SpeciesControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IUserContext> _mockUserContext;
        private IDocumentStore _documentStore;
        private SpeciesController _controller;

        [SetUp]
        public void TestInitialize()
        {
            //_documentStore = DocumentStoreHelper.StartRaven();
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockUserContext = new Mock<IUserContext>();
        }

        [TearDown]
        public void TestCleanup()
        {
            //_documentStore = null;
            //DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void SpeciesController_Constructor_With_Null_CommandProcessor()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new SpeciesController(
                        null,
                        _mockUserContext.Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void SpeciesController_Constructor_With_Null_UserContext()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new SpeciesController(
                        _mockCommandProcessor.Object,
                        null
                        )));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        #endregion 
    }
}