﻿using System;
using System.Collections.Generic;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Repositories;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Moq;

namespace Bowerbird.Core.Test.CommandHandlers
{
    [TestFixture]
    public class ObservationCreateCommandHandlerTest
    {
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

        #region Constructor tests

        [Test]
        public void ObservationCreateCommandHandler_Constructor_With_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new ObservationCreateCommandHandler(
                    null,
                    _mockUserRepository.Object,
                    _mockMediaResourceRepository.Object)));
        }

        [Test]
        public void ObservationCreateCommandHandler_Constructor_With_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new ObservationCreateCommandHandler(
                    _mockObservationRepository.Object,
                    null,
                    _mockMediaResourceRepository.Object)));
        }

        [Test]
        public void ObservationCreateCommandHandler_Constructor_With_Null_MediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new ObservationCreateCommandHandler(
                    _mockObservationRepository.Object,
                    _mockUserRepository.Object,
                    null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        public void ObservationCreateCommandHandler_Handle_Passing_Null_ObservationCreateCommandHandle_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => _observationCreateCommandHandler.Handle(null)
                ));
        }

        [Test]
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_Calls_ObservationRepository_Add()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUserEntity.Object);
            _mockObservationCreateCommand.Setup(x => x.Username).Returns(FakeValues.UserName);

            _observationCreateCommandHandler.Handle(_mockObservationCreateCommand.Object);

            _mockObservationRepository.Verify(x => x.Add(It.IsAny<Observation>()), Times.Once());
        }

        [Test]
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_Calls_UserRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUserEntity.Object);
            _mockObservationCreateCommand.Setup(x => x.Username).Returns(FakeValues.UserName);

            _observationCreateCommandHandler.Handle(_mockObservationCreateCommand.Object);

            _mockUserRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
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
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_Without_MediaResources_DoesNotCall_MediaResourceRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUserEntity.Object);
            _mockObservationCreateCommand.Setup(x => x.Username).Returns(FakeValues.UserName);

            _observationCreateCommandHandler.Handle(_mockObservationCreateCommand.Object);

            _mockMediaResourceRepository.Verify(x => x.Load(It.IsAny<IEnumerable<string>>()), Times.Never());
        }

        #endregion

        #region Helpers

        private static IEnumerable<MediaResource> TestMediaResources()
        {
            return new List<MediaResource>() { new ProxyMediaResource(FakeValues.Filename,FakeValues.FileFormat,FakeValues.Description) };
        }

        private static List<string> TestMediaResourceIds()
        {
            return new List<string>() {Guid.NewGuid().ToString()};
        }

        private class ProxyMediaResource : MediaResource
        {
            public ProxyMediaResource(string originalFileName, string fileFormat, string description)
                : base(originalFileName, fileFormat, description) { }
        }

        #endregion

    }
}