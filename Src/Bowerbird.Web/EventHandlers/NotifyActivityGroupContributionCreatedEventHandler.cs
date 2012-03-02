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
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Notifications;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityGroupContributionCreatedEventHandler : IEventHandler<DomainModelCreatedEvent<GroupContribution>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationProcessor _notificationProcessor;

        #endregion

        #region Constructors

        public NotifyActivityGroupContributionCreatedEventHandler(
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

        // if a group we're involved in has a contribution added, let's see it..
        public void Handle(DomainModelCreatedEvent<GroupContribution> @event)
        {
            Check.RequireNotNull(@event, "event");

            var usersToNotify = _documentSession
                .Query<GroupMember>()
                .Where(x => x.Group.Id == @event.DomainModel.GroupId)
                .ToList()
                .Select(x => x.User.Id);

            var activity = new Activity(@event.CreatedByUser,
                                        DateTime.Now,
                                        Nouns.Observation,
                                        Adjectives.Created,
                                        @event.DomainModel.GroupId,
                                        string.Empty,
                                        @event.EventMessage);

            _notificationProcessor.Notify(activity, usersToNotify);
        }

        #endregion
    }
}