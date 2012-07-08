using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Factories
{
    public interface IEventHandlerFactory
    {
        IEventHandler<T> Make<T>(T domainEvent) where T : IDomainEvent;
    }
}
