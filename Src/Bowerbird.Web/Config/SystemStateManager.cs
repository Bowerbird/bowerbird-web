using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.DesignByContract;
using Ninject.Activation;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Config
{
    public class SystemStateManager : ISystemStateManager
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private SystemState _cachedSystemState;
        private object _syncLock = new object();

        #endregion

        #region Constructors

        public SystemStateManager(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");
             
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        public bool FireEvents
        {
            get { return GetSystemState().FireEvents; }
        }

        public bool ExecuteCommands
        {
            get { return GetSystemState().ExecuteCommands; }
        }

        public bool SendEmails
        {
            get { return GetSystemState().SendEmails; }
        }

        public bool SystemDataSetup
        {
            get { return GetSystemState().SystemDataSetupDate != null; }
        }

        #endregion

        #region Methods

        public void SystemDataSetupDate(DateTime dateTime)
        {
            lock (_syncLock)
            {
                var systemState = GetSystemState();

                Check.Ensure(systemState.SystemDataSetupDate == null, "System data has alrready been setup.");

                systemState.SystemDataSetupDate = dateTime;
                SetSystemState(systemState);
            }
        }

        public void DisableEventProcessor()
        {
            lock (_syncLock)
            {
                var systemState = GetSystemState();
                systemState.FireEvents = false;
                SetSystemState(systemState);
            }
        }

        public void EnableEventProcessor()
        {
            lock (_syncLock)
            {
                var systemState = GetSystemState();
                systemState.FireEvents = true;
                SetSystemState(systemState);
            }
        }

        public void DisableEmailService()
        {
            lock (_syncLock)
            {
                var systemState = GetSystemState();
                systemState.SendEmails = false;
                SetSystemState(systemState);
            }
        }

        public void EnableEmailService()
        {
            lock (_syncLock)
            {
                var systemState = GetSystemState();
                systemState.SendEmails = true;
                SetSystemState(systemState);
            }
        }

        public void DisableCommandProcessor()
        {
            lock (_syncLock)
            {
                var systemState = GetSystemState();
                systemState.ExecuteCommands = false;
                SetSystemState(systemState);
            }
        }

        public void EnableCommandProcessor()
        {
            lock (_syncLock)
            {
                var systemState = GetSystemState();
                systemState.ExecuteCommands = true;
                SetSystemState(systemState);
            }
        }

        public void DisableAllServices()
        {
            lock (_syncLock)
            {
                DisableCommandProcessor();
                DisableEmailService();
                DisableEventProcessor();
            }
        }

        public void EnableAllServices()
        {
            lock (_syncLock)
            {
                EnableCommandProcessor();
                EnableEmailService();
                EnableEventProcessor();
            }
        }

        private SystemState GetSystemState()
        {
            if (_cachedSystemState == null)
            {
                _cachedSystemState = _documentSession.Load<SystemState>("settings/systemstate");

                if (_cachedSystemState == null)
                {
                    _cachedSystemState = new SystemState();
                    SetSystemState(_cachedSystemState);
                }
            }
            return _cachedSystemState;
        }

        private void SetSystemState(SystemState systemState)
        {
            _documentSession.Store(systemState);
            _documentSession.SaveChanges();
        }

        #endregion      
      
    }
}
