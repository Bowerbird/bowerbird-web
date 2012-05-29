using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Raven.Client;
using System.Dynamic;
using Bowerbird.Core.Factories;
using System.IO;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivityObservationAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Observation>>, IEventHandler<DomainModelCreatedEvent<ObservationGroup>>
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ActivityObservationAdded(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Observation> domainEvent)
        {
            dynamic activity = MakeActivity(domainEvent, "observationadded", string.Format("{0} add an observation", domainEvent.User.FirstName), domainEvent.DomainModel.Groups.Select(x => x.Group));

            activity.ObservationAdded = new
            {
                Observation = domainEvent.DomainModel
            };

            _documentSession.Store(activity);
        }

        public void Handle(DomainModelCreatedEvent<ObservationGroup> domainEvent)
        {
            dynamic activity = MakeActivity(domainEvent, "observationadded", string.Format("{0} add an observation", domainEvent.User.FirstName), new[] { domainEvent.DomainModel.Group });

            activity.ObservationAdded = new
            {
                Observation = domainEvent.Sender
            };

            _documentSession.Store(activity);

            // TODO: Notify realtime clients via signalr
        }

        private void Handle(dynamic activity)
        {

        }

        #endregion      

    }
}
