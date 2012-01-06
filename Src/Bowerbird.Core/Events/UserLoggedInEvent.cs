using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

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
            Check.RequireNotNull(user, "user");

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
