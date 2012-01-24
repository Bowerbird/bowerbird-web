using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Storage.Managed;
using Raven.Storage.Esent;

namespace Bowerbird.Test.Utils
{
    public class DocumentStoreHelper
    {
     
        public static IDocumentStore TestDocumentStore()
        {
            return new EmbeddableDocumentStore()
            {
                RunInMemory = true
            }
            .Initialize();
        }

        public static IDocumentStore LocalhostDocumentStore()
        {
            return new DocumentStore()
                       {
                           ConnectionStringName = "bowerbird-test"
                       }
                .Initialize();
        }

    }
}