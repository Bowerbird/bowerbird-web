///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//namespace Bowerbird.Test.Controllers.Members
//{
//    #region Namespaces

//    using System.Web.Mvc;
//    using NUnit.Framework;
//    using Moq;
//    using Bowerbird.Core;
//    using Bowerbird.Core.DesignByContract;
//    using Bowerbird.Test.Utils;
//    using Bowerbird.Web.Config;
//    using Bowerbird.Web.Controllers;
//    using Bowerbird.Web.ViewModels;
//    using Bowerbird.Core.CommandHandlers;
//    using Bowerbird.Web.Controllers.Members;

//    #endregion

//    [TestFixture]
//    public class AccountControllerTest
//    {
//        #region Test Infrastructure

//        private Mock<ICommandProcessor> _mockCommandProcessor;
//        private Mock<IUserContext> _mockUserContext;
//        private AccountController _controller;

//        [SetUp]
//        public void TestInitialize()
//        {
//            _mockCommandProcessor = new Mock<ICommandProcessor>();
//            _mockUserContext = new Mock<IUserContext>();

//            _controller = new AccountController(
//                _mockCommandProcessor.Object,
//                _mockUserContext.Object
//                );
//        }

//        [TearDown]
//        public void TestCleanup()
//        {
//        }

//        #endregion

//        #region Test Helpers

//        #endregion

//        #region Property tests

//        #endregion

//        #region Method tests

//        #endregion
//    }
//}