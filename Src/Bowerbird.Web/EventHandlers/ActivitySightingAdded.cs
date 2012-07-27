/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/
                
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivitySightingAdded : DomainEventHandlerBase, 
        IEventHandler<DomainModelCreatedEvent<Observation>>,
        IEventHandler<DomainModelCreatedEvent<Record>>, 
        IEventHandler<DomainModelCreatedEvent<SightingGroup>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;
        private readonly ISightingViewFactory _sightingViewFactory;

        #endregion

        #region Constructors

        public ActivitySightingAdded(
            IDocumentSession documentSession,
            IBackChannelService backChannelService,
            ISightingViewFactory sightingViewFactory
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(backChannelService, "backChannelService");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");

            _documentSession = documentSession;
            _backChannelService = backChannelService;
            _sightingViewFactory = sightingViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Observation> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationadded", 
                string.Format("{0} added an observation", domainEvent.User.GetName()), 
                domainEvent.DomainModel.Groups.Select(x => x.Group));

            activity.ObservationAdded = new
            {
                Observation = _sightingViewFactory.Make(domainEvent.DomainModel, domainEvent.User)
            };

            _documentSession.Store(activity);
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        public void Handle(DomainModelCreatedEvent<Record> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent,
                "recordadded",
                string.Format("{0} added a record", domainEvent.User.GetName()),
                domainEvent.DomainModel.Groups.Select(x => x.Group));

            activity.RecordAdded = new
            {
                Record = _sightingViewFactory.Make(domainEvent.DomainModel, domainEvent.User)
            };

            _documentSession.Store(activity);
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        public void Handle(DomainModelCreatedEvent<SightingGroup> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent,
                "sightingadded",
                string.Format("{0} added {1}", domainEvent.User.GetName(), domainEvent.Sender is Observation ? "an observation" : "a record"), 
                new[] { domainEvent.DomainModel.Group });

            activity.SightingAdded = new
            {
                Sighting = _sightingViewFactory.Make(domainEvent.Sender as Sighting, domainEvent.User)
            };

            _documentSession.Store(activity);
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        #endregion      
    }
}