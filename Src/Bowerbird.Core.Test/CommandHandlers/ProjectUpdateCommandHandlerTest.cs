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

    public class ProjectUpdateCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IRepository<Project>> _mockProjectRepository;
        private Mock<IRepository<User>> _mockUserRepository;
        private ProjectUpdateCommandHandler _projectUpdateCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockProjectRepository = new Mock<IRepository<Project>>();
            _mockUserRepository = new Mock<IRepository<User>>();
            _projectUpdateCommandHandler = new ProjectUpdateCommandHandler(
                _mockProjectRepository.Object,
                _mockUserRepository.Object
                );
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectUpdateCommandHandler_Constructor_Passing_Null_ProjectRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(()=>
                    new ProjectUpdateCommandHandler(null,_mockUserRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectUpdateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectUpdateCommandHandler(_mockProjectRepository.Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectUpdateCommandHandler_Handle_Passing_Null_ProjectUpdateCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(()=>
                    _projectUpdateCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectUpdateCommandHandler_Handle_Calls_ProjectRepository_Load()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);

            _projectUpdateCommandHandler.Handle(new ProjectUpdateCommand(){Id = FakeValues.KeyString, Description = FakeValues.Description, Name = FakeValues.Name, UserId = FakeValues.UserId});

            _mockProjectRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectUpdateCommandHandler_Handle_Calls_UserRepository_Load()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);

            _projectUpdateCommandHandler.Handle(new ProjectUpdateCommand() { Id = FakeValues.KeyString, Description = FakeValues.Description, Name = FakeValues.Name, UserId = FakeValues.UserId });

            _mockUserRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectUpdateCommandHandler_Handle_Calls_ProjectRepository_Add()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);

            _projectUpdateCommandHandler.Handle(new ProjectUpdateCommand() { Id = FakeValues.KeyString, Description = FakeValues.Description, Name = FakeValues.Name, UserId = FakeValues.UserId });

            _mockProjectRepository.Verify(x => x.Add(It.IsAny<Project>()), Times.Once());
        }

        #endregion 
    }
}