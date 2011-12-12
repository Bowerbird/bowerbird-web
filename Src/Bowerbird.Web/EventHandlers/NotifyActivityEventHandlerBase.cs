using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.EventHandlers;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;

namespace Bowerbird.Web.EventHandlers
{
    public abstract class NotifyActivityEventHandlerBase
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected void Notify(string type, User user, object data)
        {
            var activity = new Activity(
                type,
                user,
                data);

            var clients = Hub.GetClients<ActivityHub>();

            clients.activityOccurred(activity);
        }

        #endregion
         
    }
}
