using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Document;
using Raven.Client;
using Ninject.Activation;
using Raven.Client.Extensions;
using Bowerbird.Core.Entities;

namespace Bowerbird.Web.Config
{
    public class RavenDocumentStoreProvider : Provider<IDocumentStore>
    {
        protected override IDocumentStore CreateInstance(IContext ctx)
        {
            var documentStore = new DocumentStore { ConnectionStringName = "bowerbird" };


            documentStore.Conventions.FindIdentityProperty =
                                prop =>
                                    // My custom ID for a given class.
                                    //(prop.DeclaringType.IsSubclassOf(typeof(EntityWithId)) && prop.Name == "Id")
                                    (prop.DeclaringType == typeof(Role) && prop.Name == "Id")
                                    || (prop.DeclaringType == typeof(Permission) && prop.Name == "Id")
                                    // Default to general purpose.
                                    || prop.Name == "Id";

            //documentStore.Conventions.DocumentKeyGenerator = entity =>
            //{
            //    string collectionName = entity.GetType().Name.ToLower() + "s";

            //    if (!(entity is User))
            //    {
            //        collectionName += "/";
            //    }

            //    return collectionName;
            //};

            documentStore.Initialize();

            documentStore.DatabaseCommands.EnsureDatabaseExists("bowerbird_dev"); // TODO: Move into config file

            return documentStore;
        }
    }
}
