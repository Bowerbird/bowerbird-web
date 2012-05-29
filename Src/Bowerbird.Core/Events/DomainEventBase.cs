using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Events
{
    public abstract class DomainEventBase : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        protected DomainEventBase(
            User user, 
            object sender)
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(sender, "sender");

            User = user;
            Sender = sender;
        }

        #endregion

        #region Properties

        public User User { get; private set; }

        public object Sender { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}
