using System;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.EventHandlers
{
    public class SendWelcomeEmailEventHandler : IEventHandler<DomainModelCreatedEvent<User>>
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<User> userCreatedEvent)
        {
            Check.RequireNotNull(userCreatedEvent, "userCreatedEvent");

            Console.Write("send welcome email to: {0}", userCreatedEvent.DomainModel.Email);
        }

        #endregion      

    }
}
