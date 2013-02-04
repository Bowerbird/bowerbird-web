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
using Raven.Client;
using Ninject.Activation;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Infrastructure
{
    public class NinjectRavenSessionProvider : Provider<IDocumentSession>
    {

        #region Members

        private readonly IDocumentStore _documentStore;
        private readonly IConfigSettings _configSettings;

        #endregion

        #region Constructors

        public NinjectRavenSessionProvider(
            IDocumentStore documentStore,
            IConfigSettings configService)
        {
            Check.RequireNotNull(documentStore, "documentStore");
            Check.RequireNotNull(configService, "configService");

            _documentStore = documentStore;
            _configSettings = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override IDocumentSession CreateInstance(IContext ctx)
        {
            if (!string.IsNullOrWhiteSpace(_configSettings.GetDatabaseName()))
            {
                return _documentStore.OpenSession(_configSettings.GetDatabaseName());
            }
            else
            {
                return _documentStore.OpenSession();
            }
        }

        #endregion      

    }
}

