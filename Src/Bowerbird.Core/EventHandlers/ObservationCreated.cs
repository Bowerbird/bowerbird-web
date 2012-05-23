using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Raven.Client;

namespace Bowerbird.Core.EventHandlers
{
    public class ObservationCreated : IEventHandler<DomainModelCreatedEvent<Observation>>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationCreated(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Observation> args)
        {
            Activity activity = new Activity("observationcreated", DateTime.Now, args.CreatedByUser, args.DomainModel, null);

            _documentSession.Store(activity);
        }

        #endregion      

    }
}
