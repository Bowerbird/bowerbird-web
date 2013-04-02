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
using Raven.Client.Document;
using Raven.Client;
using Ninject.Activation;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Infrastructure
{
    public class NinjectRavenDocumentStoreProvider : Provider<IDocumentStore>
    {

        #region Members

        private readonly IConfigSettings _configSettings;
        private readonly RavenDocumentStoreListener _ravenDocumentStoreListener;

        #endregion

        #region Constructors

        public NinjectRavenDocumentStoreProvider(
            IConfigSettings configService,
            RavenDocumentStoreListener ravenDocumentStoreListener)
        {
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(ravenDocumentStoreListener, "ravenDocumentStoreListener");

            _configSettings = configService;
            _ravenDocumentStoreListener = ravenDocumentStoreListener;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override IDocumentStore CreateInstance(IContext ctx)
        {
            var documentStore = new DocumentStore
            {
                Url = _configSettings.GetDatabaseUrl()
            };

            var hasDefaultDatabase = !string.IsNullOrWhiteSpace(_configSettings.GetDatabaseName());

            if (hasDefaultDatabase)
            {
                documentStore.DefaultDatabase = _configSettings.GetDatabaseName();
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
            //documentStore.Conventions.CustomizeJsonSerializer = serializer =>
            //{
            //    serializer.TypeNameHandling = TypeNameHandling.None;
            //};

            documentStore.RegisterListener(_ravenDocumentStoreListener);

            documentStore.Initialize();

            if (hasDefaultDatabase)
            {
                documentStore.DatabaseCommands.EnsureDatabaseExists(_configSettings.GetDatabaseName());
            }

            IndexCreation.CreateIndexes(typeof(All_Groups).Assembly, documentStore);
            All_Activities.Create(documentStore);

            return documentStore;
        }

        #endregion      

    }
}