///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//namespace Bowerbird.Web.Test.Controllers
//{
//    #region Namespaces

//    using System;
//    using System.Web.Mvc;

//    using NUnit.Framework;
//    using Moq;

//    using Bowerbird.Core;
//    using Bowerbird.Core.DesignByContract;
//    using Bowerbird.Core.Tasks;
//    using Bowerbird.Test.Utils;
//    using Bowerbird.Web.Config;
//    using Bowerbird.Web.Controllers;
//    using Bowerbird.Web.ViewModels;
//    using Bowerbird.Core.CommandHandlers;
//    using Bowerbird.Core.Commands;

//    #endregion

//    [TestFixture]
//    public class ProjectControllerTest
//    {
//        #region Test Infrastructure

//        private Mock<ICommandProcessor> _mockCommandProcessor;
//        private Mock<IViewModelRepository> _mockViewModelRepository;
//        private Mock<IUserTasks> _mockUserTasks;
//        private Mock<IUserContext> _mockUserContext;
//        private ProjectController _controller;

//        [SetUp]
//        public void TestInitialize()
//        {
//            _mockCommandProcessor = new Mock<ICommandProcessor>();
//            _mockViewModelRepository = new Mock<IViewModelRepository>();
//            _mockUserTasks = new Mock<IUserTasks>();
//            _mockUserContext = new Mock<IUserContext>();
//            _controller = new ProjectController(
//                _mockCommandProcessor.Object,
//                _mockViewModelRepository.Object,
//                _mockUserTasks.Object,
//                _mockUserContext.Object);
//        }

//        [TearDown]
//        public void TestCleanup()
//        {
//        }

//        #endregion

//        #region Test Helpers

//        #endregion

//        #region Constructor tests

//        [Test]
//        [Category(TestCategory.Unit)]
//        public void Project_Constructor_With_Null_CommandProcessor_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(
//                BowerbirdThrows.Exception<DesignByContractException>(
//                    () => new ProjectController(
//                        null, 
//                        _mockViewModelRepository.Object,
//                        _mockUserTasks.Object,
//                        _mockUserContext.Object)));
//        }

//        [Test]
//        [Category(TestCategory.Unit)]
//        public void Project_Constructor_With_Null_ViewModelRepository_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(
//                BowerbirdThrows.Exception<DesignByContractException>(
//                    () => new ProjectController(
//                        _mockCommandProcessor.Object,
//                        null,
//                        _mockUserTasks.Object,
//                        _mockUserContext.Object)));
//        }

//        [Test]
//        [Category(TestCategory.Unit)]
//        public void Project_Constructor_With_Null_UserTasks_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(
//                BowerbirdThrows.Exception<DesignByContractException>(
//                    () => new ProjectController(
//                        _mockCommandProcessor.Object,
//                        _mockViewModelRepository.Object,
//                        null,
//                        _mockUserContext.Object)));
//        }

//        [Test]
//        [Category(TestCategory.Unit)]
//        public void Project_Constructor_With_Null_UserContext_Throws_DesignByContractException()
//        {
//            Assert.IsTrue(
//                BowerbirdThrows.Exception<DesignByContractException>(
//                    () => new ProjectController(
//                        _mockCommandProcessor.Object,
//                        _mockViewModelRepository.Object,
//                        _mockUserTasks.Object,
//                        null)));
//        }

//        #endregion

//        #region Property tests

//        #endregion

//        #region Method tests

//        [Test]
//        [Category(TestCategory.Unit)]
//        public void Project_Index_Returns_ProjectIndex_ViewModel()
//        {
//            _mockViewModelRepository
//                .Setup(x => x.Load<ProjectIndexInput, ProjectIndex>(It.IsAny<ProjectIndexInput>()))
//                .Returns(new ProjectIndex());

//            _controller.Index(new ProjectIndexInput());

//            var viewModel = _controller.ViewData.Model;

//            Assert.IsInstanceOf<HomeIndex>(viewModel);
//        }

//        #endregion
//    }
//}