using Bowerbird.Core.Commands;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Infrastructure
{
    public interface IMessageBus
    {
        void Send<T>(T command) where T : ICommand;

        void SendAsync<T>(T command) where T : ICommand;

        void Publish<T>(T @event) where T : IDomainEvent;

        void PublishAsync<T>(T @event) where T : IDomainEvent;
    }
}