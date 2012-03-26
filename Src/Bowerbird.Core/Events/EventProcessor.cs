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
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.EventHandlers;
using Raven.Client;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Config;

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

        public static void Raise<T>(T args) where T : IDomainEvent
        {
            var systemState = ServiceLocator.GetInstance<ISystemState>();

            if (systemState.FireEvents)
            {
                foreach (var handler in ServiceLocator.GetAllInstances<IEventHandler<T>>())
                {
                    handler.Handle(args);
                }

                if (_actions == null) return;

                foreach (var action in _actions)
                {
                    if (action is Action<T>)
                    {
                        ((Action<T>)action)(args);
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