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
using System.Dynamic;
using System.IO;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.Config;
using Bowerbird.Web.Services;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivityObservationAdded : DomainEventHandlerBase, 
        IEventHandler<DomainModelCreatedEvent<Observation>>, 
        IEventHandler<DomainModelCreatedEvent<ObservationGroup>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IBackChannelService _backChannelService;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly ISightingViewFactory _sightingViewFactory;

        #endregion

        #region Constructors

        public ActivityObservationAdded(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IBackChannelService backChannelService,
            IUserViewModelBuilder userViewModelBuilder,
            ISightingViewFactory sightingViewFactory
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(backChannelService, "backChannelService");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _backChannelService = backChannelService;
            _userViewModelBuilder = userViewModelBuilder;
            _sightingViewFactory = sightingViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Observation> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationadded", 
                string.Format("{0} added an observation", domainEvent.User.GetName()), 
                domainEvent.DomainModel.Groups.Select(x => x.Group));

            activity.ObservationAdded = new
            {
                Observation = _sightingViewFactory.Make(domainEvent.DomainModel, domainEvent.User)
            };

            _documentSession.Store(activity);
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        public void Handle(DomainModelCreatedEvent<ObservationGroup> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationadded", 
                string.Format("{0} added an observation", domainEvent.User.GetName()), 
                new[] { domainEvent.DomainModel.Group });

            activity.ObservationAdded = new
            {
                Observation = _sightingViewFactory.Make(domainEvent.Sender as Observation, domainEvent.User)
            };

            _documentSession.Store(activity);
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        #endregion      
    }
}