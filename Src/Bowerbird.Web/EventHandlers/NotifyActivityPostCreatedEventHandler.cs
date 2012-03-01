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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityPostCreatedEventHandler : NotifyActivityEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Post>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public NotifyActivityPostCreatedEventHandler(
            IUserContext userContext,
            IDocumentSession documentSession)
            : base(userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        // if a user in a group we're involved in creates a post, let's see it..
        public void Handle(DomainModelCreatedEvent<Post> @event)
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

            var connectedUserIds = _documentSession
                .Query<ClientSession>()
                .Where(x => x.User.Id.In(membersBelongingToSameGroups))
                .Select(x => x.ClientId.ToString())
                .ToList();

            Notify(
                new ActivityMessage()
                    {
                        GroupId = "",
                        Avatar = new Avatar()
                        {
                            AltTag = @event.CreatedByUser.GetName(),
                            UrlToImage = ""
                        },
                        Message = @event.EventMessage,
                        Sender = "postcreated"
                    }, 
                connectedUserIds);
        }

        #endregion
    }
}
