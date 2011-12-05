using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.Events
{
    public class UserLoggedInEvent : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        public UserLoggedInEvent(
            User user)
        {
            User = user;
        }

        #endregion

        #region Properties

        public User User { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}
