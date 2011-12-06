﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR.Hubs;
using Bowerbird.Core.Entities;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;
using Raven.Client;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Hubs
{
    public class ActivityHub : Hub, IEventHandler<UserLoggedInEvent>, IEventHandler<EntityCreatedEvent<Observation>>
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ActivityHub(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void StartActivityStream()
        {
            throw new NotImplementedException();
        }

        void IEventHandler<UserLoggedInEvent>.Handle(UserLoggedInEvent args)
        {
            Clients.onUserLoggedIn(args.User);
        }

        void IEventHandler<EntityCreatedEvent<Observation>>.Handle(EntityCreatedEvent<Observation> args)
        {
            Clients.observationCreated(args.Entity);
        }

        #endregion      

    }
}
