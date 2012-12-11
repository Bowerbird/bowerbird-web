/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
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
    /// Logs an activity item when a sighting is added. The situations in which this can occur are:
    /// - A new sighting is created, in which case we only add one activity item representing all groups the sighting has been added to;
    /// - A sighting being added to a group after the sighting's creation.
    /// </summary>
    public class ActivitySightingAdded : DomainEventHandlerBase,
        IEventHandler<SightingCreatedEvent>,
        IEventHandler<SightingGroupCreatedEvent>
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

        public void Handle(SightingCreatedEvent domainEvent)
        {
            Execute(domainEvent, domainEvent.DomainModel, domainEvent.Projects);
        }

        public void Handle(SightingGroupCreatedEvent domainEvent)
        {
            if (domainEvent.Group is Project)
            {
                Execute(domainEvent, domainEvent.Sender as Sighting, new [] { domainEvent.Group as Project });
            }
        }

        private void Execute(IDomainEvent domainEvent, Sighting sighting, IEnumerable<Project> projects)
        {
            dynamic activity = MakeActivity(
                domainEvent,
                "sightingadded",
                sighting.CreatedOn,
                string.Format("{0} added a sighting", domainEvent.User.GetName()),
                sighting.Groups.Select(x => x.Group));

            if (sighting is Observation)
            {
                activity.ObservationAdded = new
                {
                    Observation = _sightingViewFactory.Make(sighting, domainEvent.User, projects)
                };
            }
            else
            {
                activity.RecordAdded = new
                {
                    Record = _sightingViewFactory.Make(sighting, domainEvent.User, projects)
                };
            }

            _documentSession.Store(activity);
            _documentSession.SaveChanges();
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        #endregion      
    }
}