using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.EventHandlers;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityObservationCreatedEventHandler : NotifyActivityEventHandlerBase, IEventHandler<EntityCreatedEvent<Observation>>
    {

        #region Members

        #endregion

        #region Constructors

        public NotifyActivityObservationCreatedEventHandler(
            IUserContext userContext)
            : base(userContext)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(EntityCreatedEvent<Observation> observationCreatedEvent)
        {
            Check.RequireNotNull(observationCreatedEvent, "observationCreatedEvent");

            // TODO: Find all users that need to be notified of this new observation

            Notify(
                "observationcreated",
                observationCreatedEvent.CreatedByUser,
                observationCreatedEvent.Entity);
        }

        #endregion

    }
}
