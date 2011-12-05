using Bowerbird.Core.Events;

namespace Bowerbird.Core.EventHandlers
{
    public interface IEventHandler<in T> where T : IDomainEvent
    {
        void Handle(T args);
    }
}
