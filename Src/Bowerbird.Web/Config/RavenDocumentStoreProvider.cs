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

            documentStore.DatabaseCommands.EnsureDatabaseExists("bowerbird_dev");

            return documentStore;
        }
    }
}
