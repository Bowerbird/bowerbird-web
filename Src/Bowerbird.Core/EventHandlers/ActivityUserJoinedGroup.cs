using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Raven.Client;
using System.Dynamic;
using Bowerbird.Core.Factories;
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
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ActivityUserJoinedGroup(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Member> domainEvent)
        {
            var user = _documentSession.Load<User>(domainEvent.DomainModel.User.Id);
            var group = _documentSession.Load<dynamic>(domainEvent.DomainModel.Group.Id);

            dynamic activity = MakeActivity(domainEvent, "observationadded", string.Format("{0} joined {1}", user.FirstName, group.Name), new[] { domainEvent.DomainModel.Group });

            activity.UserJoinedGroup = new
            {
                User = user,
                Group = group
            };

            _documentSession.Store(activity);

            // TODO: Notify realtime clients via signalr
        }

        #endregion

    }
}
