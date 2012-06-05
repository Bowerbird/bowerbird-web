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

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Log an activity item when a user joins a group
    /// </summary>
    public class ActivityUserLoggedIn : DomainEventHandlerBase, IEventHandler<UserLoggedInEvent>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        public ActivityUserLoggedIn(
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

        #region Methods

        public void Handle(UserLoggedInEvent domainEvent)
        {
            _notificationService.SendUserStatusUpdate(
                new
                    {
                        domainEvent.User.Id,
                        Name = domainEvent.User.GetName(),
                        domainEvent.User.Avatar,
                        Status = 0
                    }
                );
        }

        #endregion
    }
}