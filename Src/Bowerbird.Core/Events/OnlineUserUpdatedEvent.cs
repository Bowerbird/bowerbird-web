using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class OnlineUserUpdatedEvent : DomainEventBase
    {

        #region Members

        #endregion

        #region Constructors

        public OnlineUserUpdatedEvent(
            User user,
            OnlineUser onlineUser,
            string connectionId,
            DomainModel sender)
            : base(
            user,
            sender)
        {
            OnlineUser = onlineUser;
            ConnectionId = connectionId;
        }

        #endregion

        #region Properties

        public OnlineUser OnlineUser { get; private set; }

        public string ConnectionId { get; private set; }

        #endregion

        #region Methods

        #endregion

    }
}