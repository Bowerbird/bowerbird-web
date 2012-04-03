/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Notifications;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;
using System;
using Bowerbird.Web.ViewModels.Shared;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyObservationCreatedEventHandler : IEventHandler<DomainModelCreatedEvent<Observation>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationProcessor _notificationProcessor;

        #endregion

        #region Constructors

        public NotifyObservationCreatedEventHandler(
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

        // if a user in a group we're involved in creates an observation, let's see it..
        public void Handle(DomainModelCreatedEvent<Observation> @event)
        {
            Check.RequireNotNull(@event, "event");

            var notification = new Notification()
            {
                Action = "observationcreated",
                OccurredOn = DateTime.Now,
                UserId = @event.CreatedByUser,
                Model = @event.DomainModel,
                Groups = @event.DomainModel.Groups.Select(x => x.GroupId)
            };

            _notificationProcessor.Notify(notification, (client, n) => client.observationCreated(n));
        }

        #endregion
    }
}