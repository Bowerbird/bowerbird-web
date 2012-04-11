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
using Bowerbird.Core.Services;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyObservationCreatedEventHandler : IEventHandler<DomainModelCreatedEvent<Observation>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationProcessor _notificationProcessor;
        private readonly IStreamItemFactory _streamItemFactory;
        private readonly IObservationViewFactory _observationViewFactory;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public NotifyObservationCreatedEventHandler(
            IDocumentSession documentSession,
            INotificationProcessor notificationProcessor,
            IStreamItemFactory streamItemFactory,
            IObservationViewFactory observationViewFactory,
            IMediaFilePathService mediaFilePathService)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(notificationProcessor, "notificationProcessor");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");
            Check.RequireNotNull(observationViewFactory, "observationViewFactory");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _documentSession = documentSession;
            _notificationProcessor = notificationProcessor;
            _streamItemFactory = streamItemFactory;
            _observationViewFactory = observationViewFactory;
            _mediaFilePathService = mediaFilePathService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        // if a user in a group we're involved in creates an observation, let's see it..
        public void Handle(DomainModelCreatedEvent<Observation> @event)
        {
            Check.RequireNotNull(@event, "event");

            var user = _documentSession.Load<User>(@event.CreatedByUser);

            var streamItem = _streamItemFactory.Make(
                _observationViewFactory.Make(@event.DomainModel),
                @event.DomainModel.Groups.Select(x => x.GroupId),
                "observation",
                _documentSession.Load<User>(@event.DomainModel.User.Id),
                @event.DomainModel.CreatedOn,
                @event.DomainModel.User.FirstName + " added an observation");

            var notification = new Notification()
            {
                Action = "newobservation",
                OccurredOn = DateTime.Now,
                UserId = @event.CreatedByUser,
                AvatarUri = AvatarUris.DefaultUser,
                SummaryDescription = @event.DomainModel.User.FirstName + " added an observation",
                CreatedDateTimeDescription = "just now",
                Model = new
                {
                    title = @event.DomainModel.Title,
                    imageUri = _mediaFilePathService.MakeMediaFileUri(@event.DomainModel.GetPrimaryImage().MediaResource, "large")
                }
            };

            _notificationProcessor.Notify(notification, @event.DomainModel.Groups.Select(x => x.GroupId), (client, n) => client.newNotification(n));

            _notificationProcessor.Notify(streamItem, @event.DomainModel.Groups.Select(x => x.GroupId), (client, s) => client.newStreamItem(s));
        }

        #endregion
    }
}