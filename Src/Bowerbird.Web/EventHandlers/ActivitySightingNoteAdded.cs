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
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivitySightingNoteAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<SightingNote>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IBackChannelService _backChannelService;
        private readonly ISightingViewFactory _sightingViewFactory;
        private readonly ISightingNoteViewFactory _sightingNoteViewFactory;

        #endregion

        #region Constructors

        public ActivitySightingNoteAdded(
            IDocumentSession documentSession,
            IBackChannelService backChannelService,
            ISightingViewFactory sightingViewFactory,
            ISightingNoteViewFactory sightingNoteViewFactory
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(backChannelService, "backChannelService");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");

            _documentSession = documentSession;
            _backChannelService = backChannelService;
            _sightingViewFactory = sightingViewFactory;
            _sightingNoteViewFactory = sightingNoteViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<SightingNote> domainEvent)
        {
            Check.RequireNotNull(domainEvent, "domainEvent");

            var sighting = domainEvent.Sender as Sighting;

            var descriptionActions = new List<string>();

            if(domainEvent.DomainModel.Identification != null)
            {
                descriptionActions.Add("identified");
            }
            if(domainEvent.DomainModel.Descriptions.Count() > 0)
            {
                descriptionActions.Add("described");
            }
            if (domainEvent.DomainModel.Tags.Count() > 0)
            {
                descriptionActions.Add("tagged");
            }

            string description = descriptionActions.Count() == 2 ? string.Join(" and ", descriptionActions) : 
                descriptionActions.Count() == 3 ? string.Join(", ", descriptionActions.Take(2)) + " and " + descriptionActions.Last() : 
                descriptionActions.First();

            dynamic activity = MakeActivity(
                domainEvent,
                "sightingnoteadded",
                domainEvent.DomainModel.CreatedOn,
                string.Format("{0} {1} a sighting", domainEvent.User.GetName(), description),
                sighting.Groups.Select(x => x.Group));

            var projects = _documentSession.Load<Project>(sighting.Groups.Where(x => x.Group.GroupType == "project").Select(x => x.Group.Id));

            activity.SightingNoteAdded = new
            {
                Sighting = _sightingViewFactory.Make(sighting, domainEvent.User, projects),
                SightingNote = _sightingNoteViewFactory.Make(domainEvent.DomainModel, domainEvent.User)
            };

            _documentSession.Store(activity);
            _documentSession.SaveChanges();
            _backChannelService.SendActivityToGroupChannel(activity);
        }

        #endregion      
    }
}