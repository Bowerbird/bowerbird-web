using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Commands
{
    public class CommandProcessor : ICommandProcessor
    {
        #region Members

        private readonly IServiceLocator _serviceLocator;

        #endregion

        #region Constructors

        public CommandProcessor(
            IServiceLocator serviceLocator)
        {
            Check.RequireNotNull(serviceLocator, "serviceLocator");

            _serviceLocator = serviceLocator;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            Check.RequireNotNull(command, "command");

            Validator.ValidateObject(command, new ValidationContext(command, null, null), true);

            var handlers = _serviceLocator.GetAllInstances<ICommandHandler<TCommand>>();

            if (handlers == null || !handlers.Any())
            {
                throw new CommandHandlerNotFoundException(typeof(TCommand));
            }

            foreach (var handler in handlers)
            {
                handler.Handle(command);
            }
        }

        public IEnumerable<TResult> Process<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            Check.RequireNotNull(command, "command");

            Validator.ValidateObject(command, new ValidationContext(command, null, null), true);

            var handlers = _serviceLocator.GetAllInstances<ICommandHandler<TCommand, TResult>>();

            if (handlers == null || !handlers.Any())
            {
                throw new CommandHandlerNotFoundException(typeof(TCommand));
            }

            foreach (var handler in handlers)
            {
                yield return handler.Handle(command);
            }
        }

        public void Process<TCommand, TResult>(TCommand command, Action<TResult> resultHandler) where TCommand : ICommand
        {
            Check.RequireNotNull(command, "command");

            foreach (var result in Process<TCommand, TResult>(command))
            {
                resultHandler(result);
            }
        }

        #endregion      
    }
}