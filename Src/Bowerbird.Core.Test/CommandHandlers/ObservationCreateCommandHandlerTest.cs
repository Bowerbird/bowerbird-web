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

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;
    
    #endregion

    [TestFixture]
    public class ObservationCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IRepository<Observation>> _mockObservationRepository;
        private Mock<IRepository<User>> _mockUserRepository;
        private Mock<IRepository<MediaResource>> _mockMediaResourceRepository;
        private Mock<User> _mockUserEntity;
        private Mock<ObservationCreateCommand> _mockObservationCreateCommand;
        private ICommandHandler<ObservationCreateCommand> _observationCreateCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockObservationRepository = new Mock<IRepository<Observation>>();
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockMediaResourceRepository = new Mock<IRepository<MediaResource>>();
            _mockUserEntity = new Mock<User>();
            _mockObservationCreateCommand = new Mock<ObservationCreateCommand>();
            _observationCreateCommandHandler = new ObservationCreateCommandHandler(
                _mockObservationRepository.Object,
                _mockUserRepository.Object,
                _mockMediaResourceRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static IEnumerable<MediaResource> TestMediaResources()
        {
            return new List<MediaResource>() { new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description) };
        }

        private static List<string> TestMediaResourceIds()
        {
            return new List<string>() { Guid.NewGuid().ToString() };
        }


        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Constructor_With_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationCreateCommandHandler(null,_mockUserRepository.Object,_mockMediaResourceRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Constructor_With_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationCreateCommandHandler(_mockObservationRepository.Object,null,_mockMediaResourceRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Constructor_With_Null_MediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationCreateCommandHandler(_mockObservationRepository.Object,_mockUserRepository.Object,null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Handle_Passing_Null_ObservationCreateCommandHandle_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _observationCreateCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_Calls_ObservationRepository_Add()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUserEntity.Object);
            _mockObservationCreateCommand.Setup(x => x.Username).Returns(FakeValues.UserName);

            _observationCreateCommandHandler.Handle(_mockObservationCreateCommand.Object);

            _mockObservationRepository.Verify(x => x.Add(It.IsAny<Observation>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_Calls_UserRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUserEntity.Object);
            _mockObservationCreateCommand.Setup(x => x.Username).Returns(FakeValues.UserName);

            _observationCreateCommandHandler.Handle(_mockObservationCreateCommand.Object);

            _mockUserRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_With_MediaResources_Calls_MediaResourceRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUserEntity.Object);
            _mockObservationCreateCommand.Setup(x => x.Username).Returns(FakeValues.UserName);
            _mockObservationCreateCommand.Setup(x => x.MediaResources).Returns(TestMediaResourceIds());
            _mockMediaResourceRepository.Setup(x => x.Load(It.IsAny<IEnumerable<string>>())).Returns(TestMediaResources());

            _observationCreateCommandHandler.Handle(_mockObservationCreateCommand.Object);

            _mockMediaResourceRepository.Verify(x => x.Load(It.IsAny<IEnumerable<string>>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_Without_MediaResources_DoesNotCall_MediaResourceRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUserEntity.Object);
            _mockObservationCreateCommand.Setup(x => x.Username).Returns(FakeValues.UserName);

            _observationCreateCommandHandler.Handle(_mockObservationCreateCommand.Object);

            _mockMediaResourceRepository.Verify(x => x.Load(It.IsAny<IEnumerable<string>>()), Times.Never());
        }

        #endregion
    }
}