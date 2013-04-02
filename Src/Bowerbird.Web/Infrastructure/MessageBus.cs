using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;
using Bowerbird.Core.Infrastructure;
using Ninject;
using ReflectionMagic;

namespace Bowerbird.Web.Infrastructure
{
    public class MessageBus : IMessageBus
    {
        private readonly IKernel _ninjectKernel;

        public MessageBus(
            IKernel ninjectKernel)
        {
            _ninjectKernel = ninjectKernel;
        }

        public void Send<T>(T command) where T : ICommand
        {
            var handler = _ninjectKernel.GetAll<ICommandHandler<T>>();

            if (handler == null || !handler.Any())
            {
                throw new CommandHandlerNotFoundException(typeof(T));
            }

            if (handler.Count() != 1)
            {
                throw new MultipleCommandHandlersFoundException(typeof(T));
            }

            Debug.WriteLine(string.Format("Calling commandhandler synchronously: {0}", handler.GetType().Name));
            handler.First().Handle(command);
        }

        public TResult Send<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            var handler = _ninjectKernel.GetAll<ICommandHandler<TCommand, TResult>>();

            if (handler == null || !handler.Any())
            {
                throw new CommandHandlerNotFoundException(typeof(TCommand));
            }

            if (handler.Count() != 1)
            {
                throw new MultipleCommandHandlersFoundException(typeof(TCommand));
            }

            Debug.WriteLine(string.Format("Calling commandhandler synchronously: {0}", handler.GetType().Name));
            return handler.First().HandleReturn(command);
        }

        public void SendAsync<T>(T command) where T : ICommand
        {
            var handler = _ninjectKernel.GetAll<ICommandHandler<T>>();

            if (handler == null || !handler.Any())
            {
                throw new CommandHandlerNotFoundException(typeof(T));
            }

            if (handler.Count() != 1)
            {
                throw new MultipleCommandHandlersFoundException(typeof(T));
            }

            Debug.WriteLine(string.Format("Calling commandhandler asynchronously: {0}", handler.GetType().Name));
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

            var handlers = _ninjectKernel.GetAll(type);

            foreach (var handler in handlers)
            {
                Debug.WriteLine(string.Format("Calling eventhandler synchronously: {0}", handler.GetType().Name));
                handler.AsDynamic().Handle(domainEvent);
            }
        }

        public void PublishAsync<T>(T domainEvent) where T : IDomainEvent
        {
            var type = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

            var handlers = _ninjectKernel.GetAll(type);

            foreach (var handler in handlers)
            {
                var asyncHandler = handler;

                Debug.WriteLine(string.Format("Calling eventhandler asynchronously: {0}", handler.GetType().Name));
                Task.Factory.StartNew(state => asyncHandler.AsDynamic().Handle(domainEvent), domainEvent.GetType().Name, TaskCreationOptions.LongRunning);
            }
        }
    }
}