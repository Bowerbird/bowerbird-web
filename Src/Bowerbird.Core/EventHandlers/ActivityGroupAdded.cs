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
    /// Log an activity item when a user creates a group
    /// </summary>
    public class ActivityGroupAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Group>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public ActivityGroupAdded(
            IDocumentSession documentSession,
            IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _backChannelService = backChannelService;
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
                var groups = _documentSession.Load<dynamic>(project.AncestorGroups.Select(x => x.Id));

                dynamic activity = MakeActivity(
                    domainEvent,
                    "groupadded",
                    domainEvent.DomainModel.CreatedDateTime,
                    string.Format("{0} created the {1} {2}", user.GetName(), project.Name, "project"),
                    groups);

                activity.GroupAdded = new
                {
                    User = user,
                    Group = project
                };

                _documentSession.Store(activity);
                _backChannelService.SendActivityToGroupChannel(activity);
            }

            if (domainEvent.Sender is Organisation)
            {
                var organisation = domainEvent.DomainModel as Team;
                var groups = _documentSession.Load<dynamic>(organisation.AncestorGroups.Select(x => x.Id));

                dynamic activity = MakeActivity(
                    domainEvent,
                    "groupadded",
                    domainEvent.DomainModel.CreatedDateTime,
                    string.Format("{0} created the {1} {2}", user.GetName(), organisation.Name, "organisation"),
                    groups);

                activity.GroupAdded = new
                {
                    User = user,
                    Group = organisation
                };

                _documentSession.Store(activity);
                _backChannelService.SendActivityToGroupChannel(activity);
            }
        }

        #endregion
    }
}