﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Log an activity item when a user creates a group
    /// </summary>
    public class ActivityGroupAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Group>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        public ActivityGroupAdded(
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

        public void Handle(DomainModelCreatedEvent<Group> domainEvent)
        {
            Check.RequireNotNull(domainEvent, "domainEvent");

            var user = _documentSession.Load<User>(domainEvent.DomainModel.User.Id);

            if (domainEvent.Sender is Project)
            {
                var project = domainEvent.DomainModel as Project;
                var groups = _documentSession.Load<dynamic>(project.Ancestry.Select(x => x.Id));

                dynamic activity = MakeActivity(
                    domainEvent,
                    "groupadded",
                    string.Format("{0} created the {1} {2}", user.GetName(), project.Name, "project"),
                    groups);

                activity.GroupAdded = new
                {
                    User = user,
                    Group = project
                };

                _documentSession.Store(activity);
                _notificationService.SendActivity(activity);
            }

            if (domainEvent.Sender is Team)
            {
                var team = domainEvent.DomainModel as Team;
                var groups = _documentSession.Load<dynamic>(team.Ancestry.Select(x => x.Id));

                dynamic activity = MakeActivity(
                    domainEvent,
                    "groupadded",
                    string.Format("{0} created the {1} {2}", user.GetName(), team.Name, "team"),
                    groups);

                activity.GroupAdded = new
                {
                    User = user,
                    Group = team
                };

                _documentSession.Store(activity);
                _notificationService.SendActivity(activity);
            }

            if (domainEvent.Sender is Organisation)
            {
                var organisation = domainEvent.DomainModel as Team;
                var groups = _documentSession.Load<dynamic>(organisation.Ancestry.Select(x => x.Id));

                dynamic activity = MakeActivity(
                    domainEvent,
                    "groupadded",
                    string.Format("{0} created the {1} {2}", user.GetName(), organisation.Name, "organisation"),
                    groups);

                activity.GroupAdded = new
                {
                    User = user,
                    Group = organisation
                };

                _documentSession.Store(activity);
                _notificationService.SendActivity(activity);
            }
        }

        #endregion
    }
}