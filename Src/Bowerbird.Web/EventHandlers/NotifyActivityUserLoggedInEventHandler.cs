/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Extensions;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityUserLoggedInEventHandler : NotifyActivityEventHandlerBase, IEventHandler<UserLoggedInEvent>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public NotifyActivityUserLoggedInEventHandler(
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

        // notify us when one of the users in one of our groups logs in
        public void Handle(UserLoggedInEvent @event)
        {
            Check.RequireNotNull(@event, "event");

            Check.RequireNotNull(@event, "event");

            var groupsCreatingUserBelongsTo = _documentSession
                .Query<GroupMember>()
                .Where(x => x.User.Id == @event.User.Id)
                .ToList()
                .Select(x => x.Group.Id);

            var membersBelongingToSameGroups = _documentSession
                .Query<GroupMember>()
                .Where(x => x.Group.Id.In(groupsCreatingUserBelongsTo))
                .ToList()
                .Select(x => x.User.Id)
                .Distinct();

            var connectedUserIds = _documentSession
                .Query<ClientSession>()
                .Where(x => x.User.Id.In(membersBelongingToSameGroups))
                .ToList()
                .Select(x => x.ClientId.ToString());

            Notify(
                new ActivityMessage()
                    {
                        GroupId = "",
                        Avatar = new Avatar()
                        {
                            AltTag = @event.User.GetName(),
                            UrlToImage = ""
                        },
                        Message = @event.User.GetName().AppendWith(" has logged in"),
                        Sender = "userloggedin"
                    }, 
                connectedUserIds.ToList());
        }
    
        #endregion
    }
}