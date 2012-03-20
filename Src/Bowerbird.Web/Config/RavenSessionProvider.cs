/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using Ninject.Activation;

namespace Bowerbird.Web.Config
{
    public class RavenSessionProvider : Provider<IDocumentSession>
    {

        #region Members

        private readonly IDocumentStore _documentStore;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public RavenSessionProvider(
            IDocumentStore documentStore,
            IConfigService configService)
        {
            Check.RequireNotNull(documentStore, "documentStore");
            Check.RequireNotNull(configService, "configService");

            _documentStore = documentStore;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override IDocumentSession CreateInstance(IContext ctx)
        {
            return _documentStore.OpenSession();//_configService.GetDatabaseName());
        }

        #endregion      

    }
}

