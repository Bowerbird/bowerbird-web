using System;
using System.Collections.Generic;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    /// <summary>
    /// Represents a user's "logical session" which includes all the connection Ids to all the user's SignalR clients. Please
    /// note this object is immutable to ensure easier manipulation in the OnlineUserCache (thread-safe) concurrent dictionary.
    /// </summary>
    public class OnlineUser : DomainModel
    {
        private List<string> _connectionIds;

        public OnlineUser(
            OnlineUser onlineUser,
            string connectionId,
            DateTime latestHeartbeat,
            DateTime latestInteractivity)
        {
            InitMembers();

            User = onlineUser.User;
            _connectionIds.AddRange(onlineUser.ConnectionIds);
            _connectionIds.Add(connectionId);
            LatestHeartbeat = latestHeartbeat;
            LatestInteractivity = latestInteractivity;

            ApplyEvent(new OnlineUserUpdatedEvent(onlineUser.User, this, connectionId, this));
        }

        public OnlineUser(
            User user,
            string connectionId,
            DateTime latestHeartbeat,
            DateTime latestInteractivity)
        {
            InitMembers();

            User = user;
            _connectionIds.Add(connectionId);
            LatestHeartbeat = latestHeartbeat;
            LatestInteractivity = latestInteractivity;

            ApplyEvent(new OnlineUserUpdatedEvent(user, this, connectionId, this));
        }

        public User User { get; private set; }

        public IEnumerable<string> ConnectionIds
        {
            get { return _connectionIds; }
        }

        public DateTime LatestHeartbeat { get; private set; }

        public DateTime LatestInteractivity { get; private set; }

        private void InitMembers()
        {
            _connectionIds = new List<string>();
        }
    }
}
