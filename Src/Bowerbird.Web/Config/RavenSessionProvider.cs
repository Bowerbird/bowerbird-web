using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using System.Diagnostics;
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
            return _documentStore.OpenSession(_configService.GetDatabaseName());
        }

        #endregion      

    }
}
