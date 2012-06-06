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

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Config;
using Raven.Client;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Commands
{
    public class CommandProcessor : ICommandProcessor
    {

        #region Members

        private readonly IServiceLocator _serviceLocator;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public CommandProcessor(
            IServiceLocator serviceLocator,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(serviceLocator, "serviceLocator");
            Check.RequireNotNull(documentSession, "documentSession");

            _serviceLocator = serviceLocator;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            Check.RequireNotNull(command, "command");

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

            if (appRoot.ExecuteCommands)
            {
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
        }

        public IEnumerable<TResult> Process<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            Check.RequireNotNull(command, "command");

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

            if (appRoot.ExecuteCommands)
            {
                Validator.ValidateObject(command, new ValidationContext(command, null, null), true);

                var handlers = _serviceLocator.GetAllInstances<ICommandHandler<TCommand, TResult>>();

                if (handlers == null || !handlers.Any())
                {
                    throw new CommandHandlerNotFoundException(typeof(TCommand));
                }

                foreach (var handler in handlers)
                {
                    yield return handler.HandleReturn(command);
                }
            }
        }

        public void Process<TCommand, TResult>(TCommand command, Action<TResult> resultHandler) where TCommand : ICommand
        {
            Check.RequireNotNull(command, "command");

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

            if (appRoot.ExecuteCommands)
            {
                foreach (var result in Process<TCommand, TResult>(command))
                {
                    resultHandler(result);
                }
            }
        }

        #endregion
   
    }
}