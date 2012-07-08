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
using SignalR.Hubs;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
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
    public class ActivityCommentAdded : DomainEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Comment>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public ActivityCommentAdded(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
           IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(backChannelService, "backChannelService");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Comment> domainEvent)
        {
            if(domainEvent.Sender is Observation)
            {
                var observation = _documentSession.Load<dynamic>(domainEvent.Sender.Id);

                dynamic activity = MakeActivity(
                    domainEvent, 
                    "observationcommentadded", 
                    string.Format("{0} added a comment to observation {1}", 
                    domainEvent.User.GetName(), 
                    ((Observation)observation).Title), 
                    ((Observation)observation).Groups);

                activity.ObservationCommentAdded = new
                {
                    Post = domainEvent.DomainModel
                };

                _documentSession.Store(activity);
                _backChannelService.SendActivityToGroupChannel(activity);
            }

            if(domainEvent.Sender is Post)
            {
                var post = _documentSession.Load<dynamic>(domainEvent.Sender.Id);
                var group = _documentSession.Load<dynamic>(((Post) post).GroupId);

                dynamic activity = MakeActivity(
                    domainEvent,
                    "postcommentadded",
                    string.Format("{0} added a comment to post {1}",
                    domainEvent.User.GetName(),
                    ((Post)post).Subject),
                    new []{group});

                activity.PostCommentAdded = new
                {
                    Post = domainEvent.DomainModel
                };

                _documentSession.Store(activity);
                _backChannelService.SendActivityToGroupChannel(activity);
            }
        }

        #endregion     
    }
}