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
using Raven.Client.Document;
using Raven.Client;
using Ninject.Activation;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using Bowerbird.Core.Indexes;
using System.Linq;
using Raven.Abstractions.Json;

namespace Bowerbird.Web.Config
{
    public class NinjectRavenDocumentStoreProvider : Provider<IDocumentStore>
    {

        #region Members

        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public NinjectRavenDocumentStoreProvider(
            IConfigService configService)
        {
            Check.RequireNotNull(configService, "configService");

            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override IDocumentStore CreateInstance(IContext ctx)
        {
            var documentStore = new DocumentStore
            {
                Url = _configService.GetDatabaseUrl()
            };

            var hasDefaultDatabase = !string.IsNullOrWhiteSpace(_configService.GetDatabaseName());

            if (hasDefaultDatabase)
            {
                documentStore.DefaultDatabase = _configService.GetDatabaseName();
            }

            //documentStore.Conventions.FindIdentityProperty =
            //                    prop =>
            //                        // My custom ID for a given class.
            //                        //(prop.DeclaringType.IsSubclassOf(typeof(DomainModelWithId)) && prop.Name == "Id")
            //                        //(prop.DeclaringType == typeof(Role) && prop.Name == "Id")
            //                        //|| (prop.DeclaringType == typeof(Permission) && prop.Name == "Id")
            //                        // Default to general purpose.
            //                        //prop.Name == "Id";
            //                        prop.Name == "Id";

            // Set type name handling to avoid Raven serialising type info into Activity documents. We load Activities straight out of the 
            // db and send the to the UI. They need to be clean from the outset. 
            documentStore.Conventions.CustomizeJsonSerializer = serializer =>
            {
                serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
            };

            documentStore.Initialize();

            if (hasDefaultDatabase)
            {
                documentStore.DatabaseCommands.EnsureDatabaseExists(_configService.GetDatabaseName());
            }

            IndexCreation.CreateIndexes(typeof(All_Groups).Assembly, documentStore);
            All_Activities.Create(documentStore);

            return documentStore;
        }

        #endregion      

    }
}