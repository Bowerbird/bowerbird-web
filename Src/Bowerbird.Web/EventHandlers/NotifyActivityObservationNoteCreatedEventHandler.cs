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
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;
using Bowerbird.Web.Config;
using Bowerbird.Web.Notifications;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityObservationNoteCreatedEventHandler : IEventHandler<DomainModelCreatedEvent<ObservationNote>>
    {

        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly INotificationProcessor _notificationProcessor;

        #endregion

        #region Constructors

        public NotifyActivityObservationNoteCreatedEventHandler(
            IDocumentSession documentSession,
            INotificationProcessor notificationProcessor
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(notificationProcessor, "notificationProcessor");

            _documentSession = documentSession;
            _notificationProcessor = notificationProcessor;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        // if a user adds a note to an observation belonging in one of our groups.. lets see it        
        public void Handle(DomainModelCreatedEvent<ObservationNote> @event)
        {
            Check.RequireNotNull(@event, "event");

            var observation = _documentSession.Load<Observation>(@event.DomainModel.Observation.Id);

            var observationGroups = observation.GroupContributions.Select(x => x.GroupId);

            var usersToNotify = _documentSession
                .Query<GroupMember>()
                .Where(x => x.Group.Id.In(observationGroups))
                .Select(x => x.User.Id)
                .Distinct();

            var activity = new Activity(@event.CreatedByUser,
                                        DateTime.Now,
                                        Nouns.ObservationNote,
                                        Adjectives.Created,
                                        string.Empty,
                                        string.Empty,
                                        @event.EventMessage);

            _notificationProcessor.Notify(activity, usersToNotify);
        }

        #endregion
    }
}