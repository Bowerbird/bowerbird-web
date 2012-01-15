using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class RequestPasswordResetEvent : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public User User { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
