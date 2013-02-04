using System;
using System.Linq;
using System.Threading.Tasks;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;
using Bowerbird.Core.Infrastructure;
using Microsoft.Practices.ServiceLocation;
using ReflectionMagic;

namespace Bowerbird.Web.Infrastructure
{
    public class MessageBus : IMessageBus
    {
        private readonly IServiceLocator _serviceLocator;
        //private readonly ILoggingService _loggingService;

        public MessageBus(
            IServiceLocator serviceLocator)
            //ILoggingService loggingService)
        {
            _serviceLocator = serviceLocator;
            //_loggingService = loggingService;
        }

        public void Send<T>(T command) where T : ICommand
        {
            var handler = _serviceLocator.GetAllInstances<ICommandHandler<T>>();

            if (handler == null || !handler.Any())
            {
                throw new CommandHandlerNotFoundException(typeof(T));
            }

            if (handler.Count() != 1)
            {
                throw new MultipleCommandHandlersFoundException(typeof(T));
            }

            //_loggingService.Debug(string.Format("Sending Command: {0}", command.GetType().Name));
            handler.First().Handle(command);
        }

        public void SendAsync<T>(T command) where T : ICommand
        {
            var handler = _serviceLocator.GetAllInstances<ICommandHandler<T>>();

            if (handler == null || !handler.Any())
            {
                throw new CommandHandlerNotFoundException(typeof(T));
            }

            if (handler.Count() != 1)
            {
                throw new MultipleCommandHandlersFoundException(typeof(T));
            }

            //_loggingService.Debug(string.Format("Sending Async Command: {0}", command.GetType().Name));
            Task.Factory.StartNew(state =>
                {
                    //using (IActivationBlock activation = kernel.BeginBlock())
                    //{
                        handler.First().Handle(command);
                    //}
                }, 
                command.GetType().Name);
        }

        public void Publish<T>(T domainEvent) where T : IDomainEvent
        {
            var type = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

            var handlers = _serviceLocator.GetAllInstances(type);
            //var handlers = _serviceLocator.GetAllInstances<IEventHandler<T>>();

            foreach (var handler in handlers)
            {
                //_loggingService.Debug(string.Format("Publishing Event: {0}", @event.GetType()));
                handler.AsDynamic().Handle(domainEvent);
            }
        }

        public void PublishAsync<T>(T domainEvent) where T : IDomainEvent
        {
            var type = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

            var handlers = _serviceLocator.GetAllInstances(type);

            foreach (var handler in handlers)
            {
                var asyncHandler = handler;

                //_loggingService.Debug(string.Format("Publishing Async Event: {0}", @event.GetType().Name));
                Task.Factory.StartNew(state => asyncHandler.AsDynamic().Handle(domainEvent), domainEvent.GetType().Name, TaskCreationOptions.LongRunning);
            }
        }
    }
}