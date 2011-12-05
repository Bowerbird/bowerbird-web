using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using System.Diagnostics;
using Ninject.Activation;

namespace Bowerbird.Web.Config
{
    public class RavenSessionProvider : Provider<IDocumentSession>
    {
        private readonly IDocumentStore _documentStore;

        public RavenSessionProvider(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        protected override IDocumentSession CreateInstance(IContext ctx)
        {
            Debug.Write("IDocumentSession Created");
            return _documentStore.OpenSession("bowerbird_dev");
        }
    }
}
