///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using Bowerbird.Core.Commands;
//using Bowerbird.Test.Utils;
//using Bowerbird.Web.Controllers.Public;
//using Moq;
//using NUnit.Framework;
//using Raven.Client;

//namespace Bowerbird.Test.Controllers.Public
//{
//    [TestFixture]
//    public class HomeControllerTest
//    {
//        #region Test Infrastructure

//        private Mock<ICommandProcessor> _mockCommandProcessor;
//        private IDocumentStore _documentStore;
//        private HomeController _controller;

//        [SetUp]
//        public void TestInitialize()
//        {
//            //_documentStore = DocumentStoreHelper.ServerDocumentStore();
//            _documentStore = DocumentStoreHelper.InMemoryDocumentStore();

//            _mockCommandProcessor = new Mock<ICommandProcessor>();

//            _controller = new HomeController(
//                _documentStore.OpenSession(),
//                _mockCommandProcessor.Object
//                //_documentStore.OpenSession(DocumentStoreHelper.TestDb)
//                );
//        }

//        [TearDown]
//        public void TestCleanup()
//        {
//        }

//        #endregion

//        #region Test Helpers

//        #endregion

//        #region Constructor tests

//        #endregion

//        #region Method tests

//        #endregion
//    }
//}