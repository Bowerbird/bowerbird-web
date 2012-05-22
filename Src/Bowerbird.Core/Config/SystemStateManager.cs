/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System;
using Raven.Client;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Services;

namespace Bowerbird.Core.Config
{
    public class SystemStateManager : ISystemStateManager
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IConfigService _configService;
        private static object _lock = new object();

        #endregion

        #region Constructors

        public SystemStateManager(
            IDocumentSession documentSession,
            IConfigService configService)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(configService, "configService");
             
            _documentSession = documentSession;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void SetupSystem(bool doSetupTestData)
        {
            var appRoot = LoadAppRoot();
            if (appRoot == null)
            {
                SetupSystem setupSystem = new SetupSystem(_documentSession, this, _configService);
                setupSystem.Execute();

                if (doSetupTestData)
                {
                    SetupTestData setupTestData = new SetupTestData(_documentSession, this);
                    setupTestData.Execute();
                }
            }
        }

        public void DisableEventProcessor()
        {
            lock (_lock)
            {
                var systemState = LoadAppRoot();
                systemState.SetFireEvents(false);
                SaveAppRoot(systemState);
            }
        }

        public void EnableEventProcessor()
        {
            lock (_lock)
            {
                var systemState = LoadAppRoot();
                systemState.SetFireEvents(true);
                SaveAppRoot(systemState);
            }
        }

        public void DisableEmailService()
        {
            lock (_lock)
            {
                var systemState = LoadAppRoot();
                systemState.SetSendEmails(false);
                SaveAppRoot(systemState);
            }
        }

        public void EnableEmailService()
        {
            lock (_lock)
            {
                var systemState = LoadAppRoot();
                systemState.SetSendEmails(true);
                SaveAppRoot(systemState);
            }
        }

        public void DisableCommandProcessor()
        {
            lock (_lock)
            {
                var systemState = LoadAppRoot();
                systemState.SetExecuteCommands(false);
                SaveAppRoot(systemState);
            }
        }

        public void EnableCommandProcessor()
        {
            lock (_lock)
            {
                var systemState = LoadAppRoot();
                systemState.SetExecuteCommands(true);
                SaveAppRoot(systemState);
            }
        }

        public void DisableAllServices()
        {
            lock (_lock)
            {
                DisableCommandProcessor();
                DisableEmailService();
                DisableEventProcessor();
            }
        }

        public void EnableAllServices()
        {
            lock (_lock)
            {
                EnableCommandProcessor();
                EnableEmailService();
                EnableEventProcessor();
            }
        }

        private AppRoot LoadAppRoot()
        {
            return _documentSession.Load<AppRoot>(Constants.AppRootId);
        }

        private void SaveAppRoot(AppRoot appRoot)
        {
            _documentSession.Store(appRoot);
            _documentSession.SaveChanges();
        }

        #endregion      
    }
}