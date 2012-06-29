/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Events
{
    public class EventProcessor
    {
        [ThreadStatic]
        private static List<Delegate> _actions;

        public static IServiceLocator ServiceLocator { get; set; }

        public static void ClearCallbacks()
        {
            _actions = null;
        }

        public static void Raise<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            Check.RequireNotNull(domainEvent, "domainEvent");

            var appRoot = ServiceLocator.GetInstance<IDocumentSession>().Load<AppRoot>(Constants.AppRootId);

            if (appRoot.FireEvents)
            {
                foreach (var handler in ServiceLocator.GetAllInstances<IEventHandler<TEvent>>())
                {
                    handler.Handle(domainEvent);
                }

                if (_actions == null) return;

                foreach (var action in _actions)
                {
                    if (action is Action<TEvent>)
                    {
                        ((Action<TEvent>)action)(domainEvent);
                    }
                }

            }
        }

        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (_actions == null)
            {
                _actions = new List<Delegate>();
            }
            _actions.Add(callback);
        }
    }
}