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
using Bowerbird.Web.Config;

namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityUserLoggedInEventHandler : NotifyActivityEventHandlerBase, IEventHandler<UserLoggedInEvent>
    {

        #region Members

        #endregion

        #region Constructors

        public NotifyActivityUserLoggedInEventHandler(
            IUserContext userContext)
            : base(userContext)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserLoggedInEvent userLoggedInEvent)
        {
            Check.RequireNotNull(userLoggedInEvent, "userLoggedInEvent");

            Notify(
                "userloggedin",
                userLoggedInEvent.User,
                userLoggedInEvent.User);
        }

        #endregion

    }
}
