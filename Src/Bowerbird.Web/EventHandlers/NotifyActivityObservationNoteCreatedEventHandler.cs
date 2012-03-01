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
    public class NotifyActivityObservationNoteCreatedEventHandler : NotifyActivityEventHandlerBase, IEventHandler<DomainModelCreatedEvent<ObservationNote>>
    {

        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public NotifyActivityObservationNoteCreatedEventHandler(
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

        // if a user adds a note to an observation belonging in one of our groups.. lets see it        
        public void Handle(DomainModelCreatedEvent<ObservationNote> @event)
        {
            Check.RequireNotNull(@event, "event");

            var observation = _documentSession.Load<Observation>(@event.DomainModel.Observation.Id);

            var observationGroups = observation.GroupContributions.Select(x => x.GroupId);

            var membersBelongingToSameGroups = _documentSession
                .Query<GroupMember>()
                .Where(x => x.Group.Id.In(observationGroups))
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
                    Type = "observationnotecreated"
                },
                connectedUserIds);
        }

        #endregion
				
    }
}