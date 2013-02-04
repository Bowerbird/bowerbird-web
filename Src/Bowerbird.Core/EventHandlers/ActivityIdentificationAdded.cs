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
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.ViewModelFactories;

namespace Bowerbird.Core.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivityIdentificationAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<IdentificationNew>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;
        private readonly ISightingViewFactory _sightingViewFactory;
        private readonly IIdentificationViewFactory _identificationViewFactory;

        #endregion

        #region Constructors

        public ActivityIdentificationAdded(
            IDocumentSession documentSession,
            IBackChannelService backChannelService,
            ISightingViewFactory sightingViewFactory,
            IIdentificationViewFactory identificationViewFactory
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(backChannelService, "backChannelService");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");
            Check.RequireNotNull(identificationViewFactory, "identificationViewFactory");

            _documentSession = documentSession;
            _backChannelService = backChannelService;
            _sightingViewFactory = sightingViewFactory;
            _identificationViewFactory = identificationViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<IdentificationNew> domainEvent)
        {
            Check.RequireNotNull(domainEvent, "domainEvent");

            var sighting = domainEvent.Sender as Sighting;

            dynamic activity = MakeActivity(
                domainEvent,
                "identificationadded",
                domainEvent.DomainModel.CreatedOn,
                string.Format("{0} identified a sighting", domainEvent.User.GetName()),
                sighting.Groups.Select(x => x.Group),
                sighting.Id,
                domainEvent.DomainModel.Id);

            var projects = _documentSession.Load<Project>(sighting.Groups.Where(x => x.Group.GroupType == "project").Select(x => x.Group.Id));

            activity.IdentificationAdded = new
            {
                Sighting = _sightingViewFactory.Make(sighting, domainEvent.User, projects, domainEvent.User),
                Identification = _identificationViewFactory.Make(sighting, domainEvent.DomainModel, domainEvent.User, domainEvent.User)
            };

            _documentSession.Store(activity);
            _documentSession.SaveChanges();
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        #endregion      
    }
}