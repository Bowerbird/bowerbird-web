/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityGroupContributionCreatedEventHandler : NotifyActivityEventHandlerBase, IEventHandler<DomainModelCreatedEvent<GroupContribution>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public NotifyActivityGroupContributionCreatedEventHandler(
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

        // if a group we're involved in has a contribution added, let's see it..
        public void Handle(DomainModelCreatedEvent<GroupContribution> @event)
        {
            Check.RequireNotNull(@event, "event");

            var usersBelongingToGroupContributionWasAddedTo = _documentSession
                .Query<GroupMember>()
                .Where(x => x.Group.Id == @event.DomainModel.GroupId)
                .ToList()
                .Select(x => x.User.Id);

            var connectedUserIds = _documentSession
                .Query<ClientSession>()
                .Where(x => x.User.Id.In(usersBelongingToGroupContributionWasAddedTo))
                .ToList()
                .Select(x => x.ClientId.ToString());

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
                        Type = "observationcreated"
                    }, 
                connectedUserIds.ToList());
        }

        #endregion
    }
}