/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using System.Dynamic;
using Raven.Abstractions.Linq;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Log an activity item when a user joins a group
    /// </summary>
    public class ActivityUserJoinedGroup : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Member>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        public ActivityUserJoinedGroup(
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

        public void Handle(DomainModelCreatedEvent<Member> domainEvent)
        {
            var user = _documentSession.Load<User>(domainEvent.DomainModel.User.Id);
            var group = _documentSession.Load<dynamic>(domainEvent.DomainModel.Group.Id);

            dynamic activity = MakeActivity(
                domainEvent, 
                "userjoinedgroup", 
                string.Format("{0} joined {1}", user.FirstName, group.Name), 
                new[] { domainEvent.DomainModel.Group });

            activity.UserJoinedGroup = new
            {
                User = user,
                Group = group
            };

            _documentSession.Store(activity);
            _notificationService.SendActivity(activity);
        }

        #endregion
    }
}