using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.EventHandlers
{
    public class SendWelcomeEmail : IEventHandler<EntityCreatedEvent<User>>
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(EntityCreatedEvent<User> userCreatedEvent)
        {
            Check.RequireNotNull(userCreatedEvent, "userCreatedEvent");

            Console.Write("send welcome email to: {0}", userCreatedEvent.Entity.Email);
        }

        #endregion      

    }
}
