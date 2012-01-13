/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Posts;

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

    public class ProjectPostCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IRepository<Project>> _mockProjectRepository;
        private Mock<IRepository<ProjectPost>> _mockProjectPostRepository;
        private Mock<IRepository<User>> _mockUserRepository;
        private Mock<IRepository<MediaResource>> _mockMediaResourceRepository;
        private ProjectPostCreateCommandHandler _projectPostCreateCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockProjectRepository = new Mock<IRepository<Project>>();
            _mockProjectPostRepository = new Mock<IRepository<ProjectPost>>();
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockMediaResourceRepository = new Mock<IRepository<MediaResource>>();
            _projectPostCreateCommandHandler = new ProjectPostCreateCommandHandler(
                _mockProjectRepository.Object,
                _mockProjectPostRepository.Object,
                _mockUserRepository.Object,
                _mockMediaResourceRepository.Object
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
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_Project_Repository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        null,
                        _mockProjectPostRepository.Object,
                        _mockUserRepository.Object,
                        _mockMediaResourceRepository.Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_ProjectPost_Repository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        _mockProjectRepository.Object,
                        null,
                        _mockUserRepository.Object,
                        _mockMediaResourceRepository.Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_User_Repository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        _mockProjectRepository.Object,
                        _mockProjectPostRepository.Object,
                        null,
                        _mockMediaResourceRepository.Object
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Constructor_Passing_Null_MediaResource_Repository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPostCreateCommandHandler(
                        _mockProjectRepository.Object,
                        _mockProjectPostRepository.Object,
                        _mockUserRepository.Object,
                        null
                        )));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Handle_Passing_Null_ProjectPostCreateCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    _projectPostCreateCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Handle_Calls_ProjectRepository_Load()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);
            _mockMediaResourceRepository.Setup(x => x.Load(It.IsAny<List<string>>())).Returns(new Mock<List<MediaResource>>().Object);

            var projectPostCreateCommand = new ProjectPostCreateCommand()
            {
                UserId = FakeValues.UserId,
                Timestamp = FakeValues.CreatedDateTime,
                Message = FakeValues.Message,
                ProjectId = FakeValues.KeyString,
                Subject = FakeValues.Subject,
                MediaResources = new List<string>()
            };

            _projectPostCreateCommandHandler.Handle(projectPostCreateCommand);

            _mockProjectRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Handle_Calls_UserRepository_Load()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);
            _mockMediaResourceRepository.Setup(x => x.Load(It.IsAny<List<string>>())).Returns(new Mock<List<MediaResource>>().Object);

            var projectPostCreateCommand = new ProjectPostCreateCommand()
            {
                UserId = FakeValues.UserId,
                Timestamp = FakeValues.CreatedDateTime,
                Message = FakeValues.Message,
                ProjectId = FakeValues.KeyString,
                Subject = FakeValues.Subject,
                MediaResources = new List<string>()
            };

            _projectPostCreateCommandHandler.Handle(projectPostCreateCommand);

            _mockUserRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Handle_Calls_ProjectPostRepository_Add()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);
            _mockProjectPostRepository.Setup(x => x.Add(It.IsAny<ProjectPost>())).Verifiable();
            _mockMediaResourceRepository.Setup(x => x.Load(It.IsAny<List<string>>())).Returns(new Mock<List<MediaResource>>().Object);

            var projectPostCreateCommand = new ProjectPostCreateCommand()
            {
                UserId = FakeValues.UserId,
                Timestamp = FakeValues.CreatedDateTime,
                Message = FakeValues.Message,
                ProjectId = FakeValues.KeyString,
                Subject = FakeValues.Subject,
                MediaResources = new List<string>()
            };

            _projectPostCreateCommandHandler.Handle(projectPostCreateCommand);

            _mockProjectPostRepository.Verify(x => x.Add(It.IsAny<ProjectPost>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostCreateCommandHandler_Handle_Calls_MediaResourceRepository_Add()
        {
            _mockProjectRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<Project>().Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);
            _mockProjectPostRepository.Setup(x => x.Add(It.IsAny<ProjectPost>())).Verifiable();
            _mockMediaResourceRepository.Setup(x => x.Load(It.IsAny<List<string>>())).Returns(new Mock<List<MediaResource>>().Object);

            var projectPostCreateCommand = new ProjectPostCreateCommand()
            {
                UserId = FakeValues.UserId,
                Timestamp = FakeValues.CreatedDateTime,
                Message = FakeValues.Message,
                ProjectId = FakeValues.KeyString,
                Subject = FakeValues.Subject,
                MediaResources = new List<string>()
            };

            _projectPostCreateCommandHandler.Handle(projectPostCreateCommand);

            _mockMediaResourceRepository.Verify(x => x.Load(It.IsAny<List<string>>()), Times.Once());
        }

        #endregion 
    }
}