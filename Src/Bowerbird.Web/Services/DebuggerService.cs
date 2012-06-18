
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR.Hubs;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Services
{
    public class DebuggerService : IDebuggerService
    {
        #region Fields

        private readonly IHubContext _hubContext;

        #endregion

        #region Constructors
        public DebuggerService(
            [HubContext(typeof(DebugHub))] IHubContext hubContext)
        {
            Check.RequireNotNull(hubContext, "hubContext");

            _hubContext = hubContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void DebugToClient(object output)
        {
            _hubContext.Clients.debugToClient(output);
        }

        #endregion
    }
}