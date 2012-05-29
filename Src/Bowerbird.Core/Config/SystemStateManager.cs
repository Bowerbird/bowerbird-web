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
        private readonly ICommandProcessor _commandProcessor;
        private static object _lock = new object();

        #endregion

        #region Constructors

        public SystemStateManager(
            IDocumentSession documentSession,
            IConfigService configService,
            ICommandProcessor commandProcessor)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(commandProcessor, "commandProcessor");
             
            _documentSession = documentSession;
            _configService = configService;
            _commandProcessor = commandProcessor;
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
                    SetupTestData setupTestData = new SetupTestData(_documentSession, this, _commandProcessor, _configService);
                    setupTestData.Execute();
                }
            }
        }

        public void SwitchServicesOff()
        {
            SwitchServices(false, false, false);
        }

        public void SwitchServicesOn()
        {
            SwitchServices(true, true, true);
        }

        public void SwitchServices(bool? enableEvents = null, bool? enableEmails = null, bool? enableCommands = null)
        {
            lock (_lock)
            {
                var systemState = LoadAppRoot();
                systemState.SetFireEvents(enableEvents ?? systemState.FireEvents);
                systemState.SetSendEmails(enableEmails ?? systemState.SendEmails);
                systemState.SetExecuteCommands(enableCommands ?? systemState.ExecuteCommands);
                SaveAppRoot(systemState);
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