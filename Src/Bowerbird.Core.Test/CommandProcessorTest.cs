/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;
    using Microsoft.Practices.ServiceLocation;
    using Moq;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.Extensions;

    #endregion

    [TestFixture]
    public class CommandProcessorTest
    {
        #region Test Infrastructure

        private Mock<IServiceLocator> _mockServiceLocator;
        private CommandProcessor _commandProcessor;

        [SetUp]
        public void TestInitialize()
        {
            _mockServiceLocator = new Mock<IServiceLocator>();
            _commandProcessor = new CommandProcessor(_mockServiceLocator.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void CommandProcessor_Constructor_Passing_Null_ServiceLocator_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new CommandProcessor(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void CommandProcessor_Process_Command_Passing_Null_Command_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _commandProcessor.Process<ProxyObjects.ProxyCommand>(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void CommandProcessor_Process_Command_With_No_Handlers_Found_Throws_CommandHandlerNotFoundException()
        {
            var handlers = new List<ICommandHandler<ProxyObjects.ProxyCommand>>();

            _mockServiceLocator.Setup(x => x.GetAllInstances<ICommandHandler<ProxyObjects.ProxyCommand>>()).Returns(handlers);

            Assert.IsTrue(BowerbirdThrows.Exception<CommandHandlerNotFoundException>(() => _commandProcessor.Process<ProxyObjects.ProxyCommand>(new ProxyObjects.ProxyCommand())));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void CommandProcessor_Process_Command_Calls_Handler_Handle_For_Each_Handler()
        {
            var mockHandler = new Mock<ProxyObjects.ProxyCommandHandler>();
            mockHandler.Setup(x => x.Handle(It.IsAny<ProxyObjects.ProxyCommand>())).Verifiable();

            var handlers = new List<ICommandHandler<ProxyObjects.ProxyCommand>>(){mockHandler.Object, mockHandler.Object};
            _mockServiceLocator.Setup(x => x.GetAllInstances<ICommandHandler<ProxyObjects.ProxyCommand>>()).Returns(handlers);

            _commandProcessor.Process<ProxyObjects.ProxyCommand>(new ProxyObjects.ProxyCommand());

            mockHandler.Verify(x => x.Handle(It.IsAny<ProxyObjects.ProxyCommand>()), Times.Exactly(2));
        }

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void CommandProcessor_Process_Command_Result_Passing_Null_Command_Throws_DesignByContractException()
        {
            var mockHandler = new Mock<ProxyObjects.ProxyCommandHandlerWithResult>();
            mockHandler.Setup(x => x.Handle(It.IsAny<ProxyObjects.ProxyCommand>())).Verifiable();

            var handlers = new List<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>() { mockHandler.Object, mockHandler.Object };
            _mockServiceLocator.Setup(x => x.GetAllInstances<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>()).Returns(handlers);

            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>  _commandProcessor.Process<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>(null)));
        }

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void CommandProcessor_Process_Command_Result_With_No_Handlers_Found_Throws_CommandHandlerNotFoundException()
        {
            var handlers = new List<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>();

            _mockServiceLocator.Setup(x => x.GetAllInstances<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>()).Returns(handlers);

            Assert.IsTrue(BowerbirdThrows.Exception<CommandHandlerNotFoundException>(() => _commandProcessor.Process<ProxyObjects.ProxyCommand,ProxyObjects.ProxyResult>(new ProxyObjects.ProxyCommand())));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void CommandProcessor_Process_Command_Result_Calls_Handler_Handle_For_Each_Handler()
        {
            var mockHandler = new Mock<ProxyObjects.ProxyCommandHandlerWithResult>();
            mockHandler.Setup(x => x.Handle(It.IsAny<ProxyObjects.ProxyCommand>())).Verifiable();

            var handlers = new List<ICommandHandler<ProxyObjects.ProxyCommand,ProxyObjects.ProxyResult>>() { mockHandler.Object, mockHandler.Object };
            _mockServiceLocator.Setup(x => x.GetAllInstances<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>()).Returns(handlers);

            var results = _commandProcessor.Process<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>(new ProxyObjects.ProxyCommand());

            Assert.NotNull(results);
            Assert.AreEqual(results.GetEnumeratorCount(), 2);
            mockHandler.Verify(x => x.Handle(It.IsAny<ProxyObjects.ProxyCommand>()), Times.Exactly(2));
        }

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void CommandProcessor_Process_Command_Result_With_Action_Passing_Null_Command_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _commandProcessor.Process<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>(null)));
        }

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void CommandProcessor_Process_Command_Result_With_Action_With_No_Handlers_Found_Throws_CommandHandlerNotFoundException()
        {
            var handlers = new List<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>();

            _mockServiceLocator.Setup(x => x.GetAllInstances<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>()).Returns(handlers);

            Assert.IsTrue(BowerbirdThrows.Exception<CommandHandlerNotFoundException>(() => _commandProcessor.Process<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>(new ProxyObjects.ProxyCommand())));
        }

        [Test, Ignore]
        [Category(TestCategory.Integration)]
        public void CommandProcessor_Process_Command_Result_With_Action_Calls_Handler_Handle_For_Each_Handler()
        {
            var mockHandler = new Mock<ProxyObjects.ProxyCommandHandlerWithResult>();
            mockHandler.Setup(x => x.Handle(It.IsAny<ProxyObjects.ProxyCommand>())).Verifiable();

            var handlers = new List<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>() { mockHandler.Object, mockHandler.Object };
            _mockServiceLocator.Setup(x => x.GetAllInstances<ICommandHandler<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>>()).Returns(handlers);

            _commandProcessor.Process<ProxyObjects.ProxyCommand, ProxyObjects.ProxyResult>(new ProxyObjects.ProxyCommand(), null);

            mockHandler.Verify(x => x.Handle(It.IsAny<ProxyObjects.ProxyCommand>()), Times.Exactly(2));
        }

        #endregion 
    }
}