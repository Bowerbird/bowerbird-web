using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Services;
using Microsoft.Practices.ServiceLocation;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace Bowerbird.Web.Infrastructure
{
    public class RavenDocumentStoreListener : IDocumentStoreListener
    {
        #region Members

        private readonly IMessageBus _messageBus;

        #endregion

        #region Constructors

        public RavenDocumentStoreListener(
            IMessageBus messageBus)
        {
            Check.RequireNotNull(messageBus, "messageBus");

            _messageBus = messageBus;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void AfterStore(string key, object entityInstance, RavenJObject metadata)
        {
            // After storing the document, and obtaining an ID, execute all unpublished events
            if(entityInstance is DomainModel)
            {
                DomainModel domainModel = entityInstance as DomainModel;

                foreach (var domainEvent in domainModel.GetUnPublishedEvents())
                {
                    _messageBus.Publish(domainEvent);
                }

                domainModel.MarkEventsPublished();
            }
        }

        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
        {
            return true;
        }

        #endregion

    }
}
