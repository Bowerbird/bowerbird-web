using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.Events
{
    public class EntityUpdatedEvent<T> : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        public EntityUpdatedEvent(
            T entity,
            User user)
        {
            Entity = entity;
            User = user;
        }

        #endregion

        #region Properties

        public T Entity { get; private set; }

        public User User { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
