/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;

    #endregion

    [TestFixture]
    public class ProjectPostDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IDefaultRepository<ProjectPost>> _mockProjectPostRepository;
        private ProjectPostDeleteCommandHandler _projectPostDeleteCommandHandler;
        private Mock<ProjectPost> _mockProjectPost;

        [SetUp]
        public void TestInitialize()
        {
            _mockProjectPostRepository = new Mock<IDefaultRepository<ProjectPost>>();
            _mockProjectPost = new Mock<ProjectPost>();
            _projectPostDeleteCommandHandler = new ProjectPostDeleteCommandHandler(_mockProjectPostRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostDeleteCommandHandler_Constructor_Passing_Null_ProjectPostRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostDeleteCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostDeleteCommandHandler_Handle_Passing_Null_ProjectPostDeleteCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    _projectPostDeleteCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostDeleteCommandHandler_Handle_Calls_ProjectPostRepository_Load()
        {
            _mockProjectPostRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockProjectPost.Object);

            var projectPostDeleteCommand = new ProjectPostDeleteCommand();

            _projectPostDeleteCommandHandler.Handle(projectPostDeleteCommand);

            _mockProjectPostRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }


        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostDeleteCommandHandler_Handle_Calls_ProjectPostRepository_Remove()
        {
            _mockProjectPostRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockProjectPost.Object);
            _mockProjectPostRepository.Setup(x => x.Remove(It.IsAny<ProjectPost>())).Verifiable();

            var projectPostDeleteCommand = new ProjectPostDeleteCommand();

            _projectPostDeleteCommandHandler.Handle(projectPostDeleteCommand);

            _mockProjectPostRepository.Verify(x => x.Remove(It.IsAny<ProjectPost>()), Times.Once());
        }

        #endregion 
    }
}