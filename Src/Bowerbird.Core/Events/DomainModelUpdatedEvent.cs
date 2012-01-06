using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class DomainModelUpdatedEvent<T> : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        public DomainModelUpdatedEvent(
            T domainModel,
            User user)
        {
            Check.RequireNotNull(domainModel, "domainModel");
            Check.RequireNotNull(user, "user");

            DomainModel = domainModel;
            User = user;
        }

        #endregion

        #region Properties

        public T DomainModel { get; private set; }

        public User User { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}