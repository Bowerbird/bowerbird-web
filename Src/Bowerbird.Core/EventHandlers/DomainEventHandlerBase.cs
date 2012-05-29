using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.EventHandlers
{
    public class DomainEventHandlerBase
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected Activity MakeActivity<T>(DomainModelCreatedEvent<T> domainEvent, string type, string description, IEnumerable<dynamic> groups)
        {
            return new Activity(
                type,
                DateTime.Now,
                description,
                domainEvent.User,
                groups);
        }

        #endregion

    }
}
