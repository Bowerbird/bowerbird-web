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

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ObservationUpdateCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IRepository<Observation>> _mockObservationRepository;
        private Mock<IRepository<User>> _mockUserRepository;
        private Mock<IRepository<MediaResource>> _mockMediaResourceRepository;
        private Mock<Observation> _mockObservation;
        private Mock<User> _mockUser;
        private Mock<IEnumerable<MediaResource>> _mockMediaResources;
        private ObservationUpdateCommandHandler _observationUpdateCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockObservationRepository = new Mock<IRepository<Observation>>();
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockMediaResourceRepository = new Mock<IRepository<MediaResource>>();
            _mockObservation = new Mock<Observation>();
            _mockUser = new Mock<User>();
            _mockMediaResources = new Mock<IEnumerable<MediaResource>>();
            _observationUpdateCommandHandler = new ObservationUpdateCommandHandler(_mockObservationRepository.Object,_mockUserRepository.Object, _mockMediaResourceRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private ObservationUpdateCommand TestObservationUpdateCommand()
        {
            return new ObservationUpdateCommand()
                       {
                           Address = FakeValues.Address,
                           Id = FakeValues.KeyString,
                           IsIdentificationRequired = FakeValues.IsTrue,
                           Latitude = FakeValues.Latitude,
                           Longitude = FakeValues.Longitude,
                           MediaResources = new List<string>() {"abc", "def"},
                           ObservationCategory = FakeValues.Category,
                           ObservedOn = FakeValues.CreatedDateTime,
                           Title = FakeValues.Title,
                           Username = FakeValues.UserName
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommandHandler_Constructor_Passing_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationUpdateCommandHandler(null, _mockUserRepository.Object, _mockMediaResourceRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationUpdateCommandHandler(_mockObservationRepository.Object, null, _mockMediaResourceRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommandHandler_Constructor_Passing_Null_MediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationUpdateCommandHandler(_mockObservationRepository.Object, _mockUserRepository.Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommandHandler_Handle_Passing_Null_ObservationUpdateCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _observationUpdateCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ObservationUpdateCommandHandler_Handle_Calls_UserRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUser.Object);
            _mockObservationRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockObservation.Object);
            _mockMediaResourceRepository.Setup(x => x.Load(It.IsAny<List<string>>())).Returns(_mockMediaResources.Object);

            _observationUpdateCommandHandler.Handle(TestObservationUpdateCommand());

            _mockUserRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ObservationUpdateCommandHandler_Handle_Calls_ObservationRepository_Load_And_Add()
        {
            _mockObservationRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockObservation.Object);
            _mockObservationRepository.Setup(x => x.Add(It.IsAny<Observation>())).Verifiable();

            _observationUpdateCommandHandler.Handle(TestObservationUpdateCommand());

            _mockObservationRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
            _mockObservationRepository.Verify(x => x.Add(It.IsAny<Observation>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ObservationUpdateCommandHandler_Handle_Calls_MediaResourceRepository_Load()
        {
            _mockMediaResourceRepository.Setup(x => x.Load(It.IsAny<IEnumerable<string>>())).Returns(_mockMediaResources.Object);
            _mockObservationRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockObservation.Object);
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUser.Object);

            _observationUpdateCommandHandler.Handle(TestObservationUpdateCommand());

            _mockMediaResourceRepository.Verify(x => x.Load(It.IsAny<IEnumerable<string>>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ObservationUpdateCommandHandler_Handle_Calls_Observation_UpdateDetails()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUser.Object);
            _mockObservationRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockObservation.Object);
            _mockObservation.Setup(x => x.UpdateDetails(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<List<MediaResource>>())
                ).Returns(_mockObservation.Object);

            _observationUpdateCommandHandler.Handle(TestObservationUpdateCommand());

            _mockObservation.Verify(x => x.UpdateDetails(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(),
                                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(),
                                It.IsAny<List<MediaResource>>()), Times.Once());
        }

        #endregion 
    }
}
