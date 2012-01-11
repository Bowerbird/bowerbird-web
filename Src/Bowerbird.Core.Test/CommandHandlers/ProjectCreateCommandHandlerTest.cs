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

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    public class ProjectCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IDefaultRepository<Project>> _mockProjectRepository;
        private Mock<IDefaultRepository<User>> _mockUserRepository;
        private ProjectCreateCommandHandler _projectCreateCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockProjectRepository = new Mock<IDefaultRepository<Project>>();
            _mockUserRepository = new Mock<IDefaultRepository<User>>();
            _projectCreateCommandHandler = new ProjectCreateCommandHandler(
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
        public void ProjectCreateCommandHandler_Constructor_Passing_Null_ProjectRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(()=>
                    new ProjectCreateCommandHandler(null,_mockUserRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectCreateCommandHandler(_mockProjectRepository.Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Handle_Passing_Null_ProjectCreateCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    _projectCreateCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Handle_Calls_UserRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);

            _projectCreateCommandHandler.Handle(new ProjectCreateCommand(){ Description = FakeValues.Description, Name = FakeValues.Name, UserId = FakeValues.UserId});

            _mockUserRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Handle_Calls_ProjectRepository_Add()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(new Mock<User>().Object);
            _mockProjectRepository.Setup(x => x.Add(It.IsAny<Project>())).Verifiable();

            _projectCreateCommandHandler.Handle(new ProjectCreateCommand() { Description = FakeValues.Description, Name = FakeValues.Name, UserId = FakeValues.UserId });

            _mockProjectRepository.Verify(x => x.Add(It.IsAny<Project>()), Times.Once());
        }

        #endregion 
    }
}