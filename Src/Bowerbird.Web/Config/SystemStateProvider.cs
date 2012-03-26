using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.DesignByContract;
using Ninject.Activation;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Config
{
    public class SystemStateProvider : Provider<ISystemState>
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private SystemState _cachedSystemState;

        #endregion

        #region Constructors

        public SystemStateProvider(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override ISystemState CreateInstance(IContext ctx)
        {
            if (_cachedSystemState == null)
            {
                var systemState = _documentSession.Load<SystemState>("bowerbird/systemstate");

                if (systemState == null)
                {
                    systemState = new SystemState(_documentSession);
                    _documentSession.Store(systemState);
                }

                _cachedSystemState = systemState;
            }

            return _cachedSystemState;
        }

        #endregion      
      
    }
}
