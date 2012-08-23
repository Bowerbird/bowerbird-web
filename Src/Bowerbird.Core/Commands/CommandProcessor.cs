///* Bowerbird V1 

// Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Collections.Generic;
//using Microsoft.Practices.ServiceLocation;
//using Bowerbird.Core.CommandHandlers;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.Config;
//using Raven.Client;
//using Bowerbird.Core.DomainModels;
//using NLog;
//using System.Threading.Tasks;

//namespace Bowerbird.Core.Commands
//{
//    public class CommandProcessor : ICommandProcessor
//    {

//        #region Members

//        private Logger _logger = LogManager.GetLogger("CommandProcessor");
//        private readonly IServiceLocator _serviceLocator;
//        private readonly IDocumentSession _documentSession;

//        #endregion

//        #region Constructors

//        public CommandProcessor(
//            IServiceLocator serviceLocator,
//            IDocumentSession documentSession)
//        {
//            Check.RequireNotNull(serviceLocator, "serviceLocator");
//            Check.RequireNotNull(documentSession, "documentSession");

//            _serviceLocator = serviceLocator;
//            _documentSession = documentSession;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public void Process<TCommand>(TCommand command) where TCommand : ICommand
//        {
//            Check.RequireNotNull(command, "command");

//            Validator.ValidateObject(command, new ValidationContext(command, null, null), true);

//            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

//            System.Diagnostics.Debug.WriteLine("HTTP Request Document Session: {0} HasChanges: {1}, NumberOfRequests: {2}.", ((Raven.Client.Document.DocumentSession)_documentSession).Id, _documentSession.Advanced.HasChanges, _documentSession.Advanced.NumberOfRequests);

//            var handlers = _serviceLocator.GetAllInstances<ICommandHandler<TCommand>>();

//            if (handlers == null || !handlers.Any())
//            {
//                throw new CommandHandlerNotFoundException(typeof(TCommand));
//            }

//            foreach (var handler in handlers)
//            {
//                // HACK: Temp code to test the idea of async commandhandlers in the chat area
//                if (command is ChatCreateCommand || command is ChatUpdateCommand || command is ChatDeleteCommand || command is ChatMessageCreateCommand)
//                {
//                    Task.Factory.StartNew(() =>
//                    {
//                        _logger.Debug("Executing command '{0}' using command handler '{1}' in new thread", command.GetType().Name, handler.GetType().Name);
//                        handler.Handle(command);
//                    })
//                    .LogExceptions();
//                }
//                else
//                {
//                    handler.Handle(command);
//                }
//            }
//        }

//        public IEnumerable<TResult> Process<TCommand, TResult>(TCommand command) where TCommand : ICommand
//        {
//            Check.RequireNotNull(command, "command");

//            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

//            Validator.ValidateObject(command, new ValidationContext(command, null, null), true);

//            var handlers = _serviceLocator.GetAllInstances<ICommandHandler<TCommand, TResult>>();

//            if (handlers == null || !handlers.Any())
//            {
//                throw new CommandHandlerNotFoundException(typeof(TCommand));
//            }

//            foreach (var handler in handlers)
//            {
//                yield return handler.HandleReturn(command);
//            }
//        }

//        public void Process<TCommand, TResult>(TCommand command, Action<TResult> resultHandler) where TCommand : ICommand
//        {
//            Check.RequireNotNull(command, "command");

//            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

//            foreach (var result in Process<TCommand, TResult>(command))
//            {
//                resultHandler(result);
//            }
//        }

//        #endregion
   
//    }

//    public static class TaskExtensions
//    {
//        public static Task LogExceptions(this Task task)
//        {
//            task.ContinueWith(t =>
//            {
//                Logger logger = LogManager.GetLogger("CommandProcessor");
//                var aggException = t.Exception.Flatten();
//                foreach (var exception in aggException.InnerExceptions)
//                {
//                    logger.ErrorException("An error occurred in an asynchronous call", exception);
//                }

//            },
//            TaskContinuationOptions.OnlyOnFaulted);

//            return task;
//        }
//    }

//}
