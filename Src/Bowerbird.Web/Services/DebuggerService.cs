
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR.Hubs;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.DesignByContract;
using SignalR;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Services
{
    public class DebuggerService : IDebuggerService
    {
        #region Fields

        private readonly IConnectionManager _connectionManager;

        #endregion

        #region Constructors

        public DebuggerService(
            IConnectionManager connectionManager)
        {
            Check.RequireNotNull(connectionManager, "connectionManager");

            _connectionManager = connectionManager;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void DebugToClient(object output)
        {
            _connectionManager.GetHubContext<DebugHub>().Clients.debugToClient(output);
        }

        #endregion
    }
}