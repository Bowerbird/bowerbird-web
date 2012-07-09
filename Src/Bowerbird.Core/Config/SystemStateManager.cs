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
using Bowerbird.Core.Factories;

namespace Bowerbird.Core.Config
{
    public class SystemStateManager : ISystemStateManager
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IConfigSettings _configSettings;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IAvatarFactory _avatarFactory;
        private static object _lock = new object();

        #endregion

        #region Constructors

        public SystemStateManager(
            IDocumentSession documentSession,
            IConfigSettings configService,
            ICommandProcessor commandProcessor,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(avatarFactory, "avatarFactory");
             
            _documentSession = documentSession;
            _configSettings = configService;
            _commandProcessor = commandProcessor;
            _avatarFactory = avatarFactory;
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
                SetupSystem setupSystem = new SetupSystem(_documentSession, this, _configSettings, _avatarFactory, _commandProcessor);
                setupSystem.Execute();

                if (doSetupTestData)
                {
                    SetupTestData setupTestData = new SetupTestData(_documentSession, this, _commandProcessor, _configSettings, _avatarFactory);
                    setupTestData.Execute();
                }
            }
        }

        public void SwitchServicesOff()
        {
            SwitchServices(false, false);
        }

        public void SwitchServicesOn()
        {
            SwitchServices(true, true);
        }

        public void SwitchServices(bool? enableEmailService = null, bool? enableBackChannelService = null)
        {
            lock (_lock)
            {
                var appRoot = LoadAppRoot();
                appRoot.SetEmailServiceStatus(enableEmailService ?? appRoot.EmailServiceStatus);
                appRoot.SetBackChannelServiceStatus(enableBackChannelService ?? appRoot.BackChannelServiceStatus);
                SaveAppRoot(appRoot);
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