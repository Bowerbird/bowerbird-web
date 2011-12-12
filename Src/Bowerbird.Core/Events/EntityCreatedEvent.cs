using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.Events
{
    public class EntityCreatedEvent<T> : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        public EntityCreatedEvent(
            T entity,
            User createdByUser)
        {
            Entity = entity;
            CreatedByUser = createdByUser;
        }

        #endregion

        #region Properties

        public T Entity { get; private set; }

        public User CreatedByUser { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}
