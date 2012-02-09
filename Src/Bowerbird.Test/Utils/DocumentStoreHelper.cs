using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using System.Configuration;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using Raven.Storage.Managed;
using Raven.Storage.Esent;

namespace Bowerbird.Test.Utils
{
    public class DocumentStoreHelper
    {

        public const string TestDb = "bowerbird_test";

        public static IDocumentStore TestDocumentStore(bool createIndexes = true)
        {
            var documentStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true,
                Conventions = new DocumentConvention()
                                  {
                                      DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites
                                  }
            }
            .Initialize();

            if (createIndexes) IndexCreation.CreateIndexes(typeof(All_Members).Assembly, documentStore);

            return documentStore;
        }

        public static IDocumentStore LocalhostDocumentStore(bool createIndexes = true)
        {
            var documentStore = new DocumentStore { Url = "http://zen:8080/", DefaultDatabase = TestDb};

            documentStore.Conventions.FindIdentityProperty =
                                prop =>
                                    // My custom ID for a given class.
                                    //(prop.DeclaringType.IsSubclassOf(typeof(DomainModelWithId)) && prop.Name == "Id")
                                    //(prop.DeclaringType == typeof(Role) && prop.Name == "Id")
                                    //|| (prop.DeclaringType == typeof(Permission) && prop.Name == "Id")
                                    // Default to general purpose.
                                    //prop.Name == "Id";
                                    prop.Name == "Id";

            documentStore.Initialize();

            documentStore.DatabaseCommands.EnsureDatabaseExists(TestDb);

            if(createIndexes)IndexCreation.CreateIndexes(typeof(All_Members).Assembly, documentStore);

            return documentStore;

        }

    }
}