using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Repositories;
using NUnit.Framework;
using Moq;

namespace Bowerbird.Core.Test.CommandHandlers
{
    [TestFixture]
    public class ObservationCreateCommandHandlerTest
    {
        private Mock<IRepository<Observation>> _mockObservationRepository;
        private Mock<IRepository<User>> _mockUserRepository;
        private ICommandHandler<ObservationCreateCommand> _observationCreateCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockObservationRepository = new Mock<IRepository<Observation>>();
            _mockUserRepository = new Mock<IRepository<User>>();

            _observationCreateCommandHandler = new ObservationCreateCommandHandler(
                _mockObservationRepository.Object,
                _mockUserRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #region Constructor tests

        [Test]
        public void ObservationCreateCommandHandler_Constructor_With_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                () => new ObservationCreateCommandHandler(
                    null,
                    _mockUserRepository.Object)));
        }

        [Test]
        public void ObservationCreateCommandHandler_Constructor_With_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                () => new ObservationCreateCommandHandler(
                    _mockObservationRepository.Object,
                    null)));
        }

        #endregion

        #region Property tests


        #endregion

        #region Method tests


        #endregion

        #region Helpers


        #endregion
				
    }
}