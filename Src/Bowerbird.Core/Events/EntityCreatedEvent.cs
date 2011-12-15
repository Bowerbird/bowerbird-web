using Bowerbird.Core.DesignByContract;
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
            Check.RequireNotNull(entity, "entity");
            Check.RequireNotNull(createdByUser, "createdByUser");
            
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
