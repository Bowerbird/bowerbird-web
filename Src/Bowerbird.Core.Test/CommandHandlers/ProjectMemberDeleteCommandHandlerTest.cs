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

    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;

    #endregion

    public class ProjectMemberDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IProjectMemberRepository> _mockProjectMemberRepository;
        private ProjectMemberDeleteCommandHandler _projectMemberDeleteCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockProjectMemberRepository = new Mock<IProjectMemberRepository>();
            _projectMemberDeleteCommandHandler = new ProjectMemberDeleteCommandHandler(_mockProjectMemberRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberDeleteCommandHandler_Constructor_Passing_Null_ProjectMemberRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(()=>
                    new ProjectMemberDeleteCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberDeleteCommandHandler_Handle_Passing_Null_ProjectMemberDeleteCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    _projectMemberDeleteCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberDeleteCommandHandler_Handle_Calls_ProjectMemberRepository_Load()
        {
            _mockProjectMemberRepository.Setup(x => x.Load(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            _projectMemberDeleteCommandHandler.Handle(new ProjectMemberDeleteCommand()
                                                          {
                                                              DeletedByUserId = FakeValues.UserId,
                                                              ProjectId = FakeValues.KeyString,
                                                              UserId = FakeValues.UserId
                                                          });

            _mockProjectMemberRepository.Verify(x => x.Load(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberDeleteCommandHandler_Handle_Calls_ProjectMemberRepository_Remove()
        {
            _mockProjectMemberRepository.Setup(x => x.Load(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            _mockProjectMemberRepository.Setup(x => x.Remove(It.IsAny<ProjectMember>())).Verifiable();

            _projectMemberDeleteCommandHandler.Handle(new ProjectMemberDeleteCommand()
            {
                DeletedByUserId = FakeValues.UserId,
                ProjectId = FakeValues.KeyString,
                UserId = FakeValues.UserId
            });

            _mockProjectMemberRepository.Verify(x => x.Remove(It.IsAny<ProjectMember>()), Times.Once());
        }

        #endregion 
    }
}