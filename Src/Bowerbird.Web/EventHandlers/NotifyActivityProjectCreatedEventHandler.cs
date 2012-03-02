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
using Bowerbird.Core.Commands;
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
    public class NotifyActivityProjectCreatedEventHandler : IEventHandler<DomainModelCreatedEvent<Project>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationProcessor _notificationProcessor;
        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public NotifyActivityProjectCreatedEventHandler(
            IDocumentSession documentSession,
            INotificationProcessor notificationProcessor,
            ICommandProcessor commandProcessor)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(notificationProcessor, "notificationProcessor");
            Check.RequireNotNull(commandProcessor, "commandProcessor");

            _documentSession = documentSession;
            _commandProcessor = commandProcessor;
            _notificationProcessor = notificationProcessor;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        // if a user in a group we're involved in creates a project, let's see it..
        public void Handle(DomainModelCreatedEvent<Project> @event)
        {
            Check.RequireNotNull(@event, "event");

            var groupsCreatingUserBelongsTo = _documentSession
                .Query<GroupMember>()
                .Where(x => x.User.Id == @event.CreatedByUser.Id)
                .ToList()
                .Select(x => x.Group.Id);

            var membersBelongingToSameGroups = _documentSession
                .Query<GroupMember>()
                .Where(x => x.Group.Id.In(groupsCreatingUserBelongsTo))
                .Select(x => x.User.Id)
                .Distinct();

            var activity = new Activity(@event.CreatedByUser,
                                        DateTime.Now,
                                        Nouns.Project,
                                        Adjectives.Created,
                                        string.Empty,
                                        string.Empty,
                                        @event.EventMessage);

            _commandProcessor.Process(new NotificationCreatedCommand()
            {
                Activity = activity,
                Timestamp = DateTime.Now,
                UserIds = membersBelongingToSameGroups
            });

            _notificationProcessor.Notify(activity, membersBelongingToSameGroups);
        }

        #endregion
    }
}