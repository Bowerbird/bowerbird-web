using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Raven.Client;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Config
{
    public class SystemState : ISystemState
    {

        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SystemState(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;

            Id = "bowerbird/systemstate";
        }

        #endregion

        #region Properties

        public string Id { get; private set; }

        public DateTime? CoreDataSetupDate { get; private set; }

        public bool FireEvents { get; private set; }

        public bool SendEmails { get; private set; }

        public bool ExecuteCommands { get; private set; }

        #endregion

        #region Methods

        public ISystemState SetCoreDataSetupDate(DateTime dateTime)
        {
            CoreDataSetupDate = dateTime;
            Persist();
            return this;
        }

        public ISystemState TurnEventsOn()
        {
            FireEvents = true;
            Persist();
            return this;
        }

        public ISystemState TurnEventsOff()
        {
            FireEvents = false;
            Persist();
            return this;
        }

        private void Persist()
        {
            _documentSession.Store(this);
        }

        #endregion   
      
    }
}