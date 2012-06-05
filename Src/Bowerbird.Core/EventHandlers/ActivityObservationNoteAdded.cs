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

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivityObservationNoteAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<ObservationNote>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        public ActivityObservationNoteAdded(
            IDocumentSession documentSession,
            INotificationService notificationService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(notificationService, "notificationService");

            _documentSession = documentSession;
            _notificationService = notificationService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<ObservationNote> domainEvent)
        {
            var observation = _documentSession.Load<Observation>(domainEvent.DomainModel.Observation.Id);
            
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationnoteadded", 
                string.Format("{0} added an observation note", domainEvent.User.GetName()), 
                observation.Groups.Select(x => x.Group));

            activity.ObservationNoteAdded = new
            {
                ObservationNote = domainEvent.DomainModel
            };

            _documentSession.Store(activity);
            _notificationService.SendActivity(activity);
        }

        #endregion      
    }
}