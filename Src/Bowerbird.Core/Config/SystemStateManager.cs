/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Factories;
using Bowerbird.Core.Infrastructure;
using Raven.Client;

namespace Bowerbird.Core.Config
{
    public class SystemStateManager : ISystemStateManager
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IDocumentStore _documentStore;
        private readonly IConfigSettings _configSettings;
        private readonly IMessageBus _messageBus;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private static object _lock = new object();

        #endregion

        #region Constructors

        public SystemStateManager(
            IDocumentSession documentSession,
            IDocumentStore documentStore,
            IConfigSettings configService,
            IMessageBus messageBus,
            IMediaResourceFactory mediaResourceFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(documentStore, "documentStore");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
             
            _documentSession = documentSession;
            _documentStore = documentStore;
            _configSettings = configService;
            _messageBus = messageBus;
            _mediaResourceFactory = mediaResourceFactory;
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
                SetupSystem setupSystem = new SetupSystem(this, _configSettings, _mediaResourceFactory, _messageBus, _documentStore);
                setupSystem.Execute();
            }
        }

        public void SwitchServicesOff()
        {
            SwitchServices(false, false, false, false, false, false, false);
        }

        public void SwitchServicesOn()
        {
            SwitchServices(true, true, true, true, true, true, true);
        }

        public void SwitchServices(
            bool? enableEmailService = null, 
            bool? enableBackChannelService = null, 
            bool? enableImageService = null, 
            bool? enableYouTubeVideoService = null, 
            bool? enableVimeoVideoService = null,
            bool? enableDocumentService = null,
            bool? enableAudioService = null
            )
        {
            lock (_lock)
            {
                var appRoot = LoadAppRoot();
                appRoot.SetEmailServiceStatus(enableEmailService ?? appRoot.EmailServiceStatus);
                appRoot.SetBackChannelServiceStatus(enableBackChannelService ?? appRoot.BackChannelServiceStatus);
                appRoot.SetImageServiceStatus(enableImageService ?? appRoot.ImageServiceStatus);
                appRoot.SetYouTubeVideoServiceStatus(enableYouTubeVideoService ?? appRoot.YouTubeVideoServiceStatus);
                appRoot.SetVimeoVideoServiceStatus(enableVimeoVideoService ?? appRoot.VimeoVideoServiceStatus);
                appRoot.SetDocumentServiceStatus(enableDocumentService ?? appRoot.DocumentServiceStatus);
                appRoot.SetAudioServiceStatus(enableAudioService ?? appRoot.AudioServiceStatus);
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