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
using Bowerbird.Web.Queries;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyObservationCreatedEventHandler : IEventHandler<DomainModelCreatedEvent<Observation>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationProcessor _notificationProcessor;
        private readonly IStreamItemFactory _streamItemFactory;
        private readonly IObservationViewFactory _observationViewFactory;

        #endregion

        #region Constructors

        public NotifyObservationCreatedEventHandler(
            IDocumentSession documentSession,
            INotificationProcessor notificationProcessor,
            IStreamItemFactory streamItemFactory,
            IObservationViewFactory observationViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(notificationProcessor, "notificationProcessor");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");

            _documentSession = documentSession;
            _notificationProcessor = notificationProcessor;
            _streamItemFactory = streamItemFactory;
            _observationViewFactory = observationViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        // if a user in a group we're involved in creates an observation, let's see it..
        public void Handle(DomainModelCreatedEvent<Observation> @event)
        {
            Check.RequireNotNull(@event, "event");

            foreach(var observationGroup in @event.DomainModel.Groups.Where(x => x.GroupType == "project"))
            {
                var streamItem = _streamItemFactory.Make(
                    _observationViewFactory.Make(@event.DomainModel),
                    "observation",
                    observationGroup.User.Id,
                    observationGroup.CreatedDateTime,
                    observationGroup.User.FirstName + " added an observation");

                var notification = new Notification()
                {
                    Action = "observationaddedtogroup",
                    OccurredOn = DateTime.Now,
                    UserId = @event.CreatedByUser,
                    Model = streamItem,
                    Groups = @event.DomainModel.Groups.Select(x => x.GroupId)
                };

                _notificationProcessor.Notify(notification, (client, n) => client.observationAddedToGroup(n));
            }
        }

        #endregion
    }
}