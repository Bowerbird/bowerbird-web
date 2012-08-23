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
using System.Collections.Generic;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public abstract class DomainModel
    {
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();

        public string Id { get; protected set; }

        public IEnumerable<IDomainEvent> GetUnPublishedEvents()
        {
            return _events;
        }

        public void MarkEventsPublished()
        {
            _events.Clear();
        }

        protected void ApplyEvent<T>(T @event) where T : IDomainEvent
        {
            _events.Add(@event);
        }

        [Obsolete]
        protected void EnableEvents()
        {
        }

        [Obsolete]
        protected void FireEvent<T>(T domainEvent) where T : IDomainEvent
        {
        }

    }
}