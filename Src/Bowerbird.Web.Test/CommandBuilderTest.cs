/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Test
{
    #region Namespaces

    using System;

    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Moq;

    using Bowerbird.Web;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Commands;
    using Bowerbird.Web.CommandFactories;
    using Bowerbird.Web.ViewModels;

    #endregion

    [TestFixture] 
    public class CommandBuilderTest
    {
        #region Test Infrastructure

        private Mock<IServiceLocator> _mockServiceLocator;
        private CommandBuilder _commandBuilder;

        [SetUp] 
        public void TestInitialize()
        {
            _mockServiceLocator = new Mock<IServiceLocator>();

            _commandBuilder = new CommandBuilder(_mockServiceLocator.Object);
        }

        [TearDown] 
        public void TestCleanup()
        {
 }

        #endregion

        #region Test Helpers

        private bool TestCommandAction()
        {
            return false; 
        }

        private ObservationCreateInput TestObservationCreateInput()
        {
            return new ObservationCreateInput()
            {
                Address = FakeValues.Address,
                Description = FakeValues.Description,
                IsIdentificationRequired = FakeValues.IsTrue,
                Latitude = FakeValues.Latitude,
                Longitude = FakeValues.Longitude,
                ObservationCategory = FakeValues.Category,
                ObservedOn = FakeValues.CreatedDateTime,
                Title = FakeValues.Title,
                UserId = FakeValues.UserId
            };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)] 
        public void CommandBuilder_Constructor_Passing_Null_ServiceLocator_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new CommandBuilder(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)] 
        public void CommandBuilder_Build_Passing_Null_Input_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    _commandBuilder.Build<object, object>(null, x => TestCommandAction())));
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void CommandBuilder_Build_Passing_Input_And_Having_ServiceLocator_Not_Find_Instance_Throws_Exception()
        {
            var input = TestObservationCreateInput();

            ICommandFactory<ObservationCreateInput, ObservationCreateCommand> commandFactory = null;

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(commandFactory);

            Assert.IsTrue(
                BowerbirdThrows.Exception<Exception>(() =>
                    _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, x => x.IsIdentificationRequired = FakeValues.IsFalse)));
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void CommandBuilder_Build_Passing_Input_And_Null_Action_Returns_Command()
        {
            var input = TestObservationCreateInput();

            var mockCommandFactory = new Mock<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>();

            mockCommandFactory
                .Setup(x => x.Make(It.IsAny<ObservationCreateInput>()))
                .Returns(new ObservationCreateCommand());

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(mockCommandFactory.Object);

            var command = _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, null);

            Assert.IsInstanceOf<ObservationCreateCommand>(command);
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void CommandBuilder_Build_Calls_Factory_Make()
        {
            var input = TestObservationCreateInput();

            var mockCommandFactory = new Mock<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>();

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(mockCommandFactory.Object);

            var command = _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, null);

            mockCommandFactory.Verify(x => x.Make(It.IsAny<ObservationCreateInput>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void CommandBuilder_Build_Passing_Input_And_Action_Invokes_Action_And_Sets_Property()
        {
            var input = TestObservationCreateInput();

            var mockCommandFactory = new Mock<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>();

            var fakeCommand = new ObservationCreateCommand()
                              {
                                  IsIdentificationRequired = false
                              };

            mockCommandFactory
                .Setup(x => x.Make(It.IsAny<ObservationCreateInput>()))
                .Returns(fakeCommand);

            _mockServiceLocator.Setup(x => x.GetInstance<ICommandFactory<ObservationCreateInput, ObservationCreateCommand>>()).Returns(mockCommandFactory.Object);

            var command = _commandBuilder.Build<ObservationCreateInput, ObservationCreateCommand>(input, x => x.IsIdentificationRequired = true);

            Assert.AreEqual(true, command.IsIdentificationRequired);
        }

        #endregion					
    }
}