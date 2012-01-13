/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Test.Controllers
{
    #region Namespaces

    using System;
    using System.Web.Mvc;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Tasks;
    using Bowerbird.Test.Utils;
    using Bowerbird.Web.Config;
    using Bowerbird.Web.Controllers;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.Commands;

    #endregion

    [TestFixture]
    public class ProjectControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private Mock<IViewModelRepository> _mockViewModelRepository;
        private ProjectController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _mockViewModelRepository = new Mock<IViewModelRepository>();
            _controller = new ProjectController(
                _mockCommandProcessor.Object,
                _mockViewModelRepository.Object);
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Constructor_With_Null_CommandProcessor_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new ProjectController(null, _mockViewModelRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_Constructor_With_Null_ViewModelRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => new ProjectController(_mockCommandProcessor.Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Project_()
        {

        }

        #endregion
    }
}