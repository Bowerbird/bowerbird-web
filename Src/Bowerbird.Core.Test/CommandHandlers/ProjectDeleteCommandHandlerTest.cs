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
    #region Namespace

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    public class ProjectDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IRepository<Project>> _mockProjectRepository;
        private ProjectDeleteCommandHandler _projectDeleteCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockProjectRepository = new Mock<IRepository<Project>>();
            _projectDeleteCommandHandler = new ProjectDeleteCommandHandler(_mockProjectRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectDeleteCommandHandler_Constructor_Passing_Null_ProjectRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(()=>
                    new ProjectDeleteCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectDeleteCommandHandler_Handle_Passing_Null_ProjectDeleteCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    _projectDeleteCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectDeleteCommandHandler_Handle_Calls_ProjectRepository_Load()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockProjectRepository.Setup(x => x.Remove(It.IsAny<Project>())).Verifiable();

            _projectDeleteCommandHandler.Handle(new ProjectDeleteCommand() { Id = FakeValues.KeyString });

            _mockProjectRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectDeleteCommandHandler_Handle_Calls_ProjectRepository_Remove()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockProjectRepository.Setup(x => x.Remove(It.IsAny<Project>())).Verifiable();

            _projectDeleteCommandHandler.Handle(new ProjectDeleteCommand(){Id = FakeValues.KeyString});

            _mockProjectRepository.Verify(x => x.Remove(It.IsAny<Project>()), Times.Once());
        }

        #endregion
    }
}