using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
using Raven.Client;

namespace Bowerbird.Core.Config
{
    public class OnlineUserCache : IOnlineUserCache
    {
        private readonly IDocumentSession _documentSession;
        private static ConcurrentDictionary<string, OnlineUser> _onlineUsers;
        private readonly IMessageBus _messageBus;

        public OnlineUserCache(
            IDocumentSession documentSession,
            IMessageBus messageBus)
        {
            _documentSession = documentSession;
            _messageBus = messageBus;

            _onlineUsers = new ConcurrentDictionary<string, OnlineUser>();
        }

        public void AddUserSession(string userId, string connectionId)
        {
            var onlineUser = new OnlineUser(
                _documentSession.Load<User>(userId),
                connectionId,
                DateTime.UtcNow,
                DateTime.UtcNow);

            UpdateCache(userId, onlineUser);
        }

        public IEnumerable<OnlineUser> CurrentlyOnlineUsers(DateTime since)
        {
            return _onlineUsers
                .Where(x => x.Value.LatestHeartbeat > since)
                .Select(x => x.Value);
        }

        public void UpdateUserSession(string userId, string connectionId, DateTime latestHeartbeat, DateTime latestInteractivity)
        {
            OnlineUser onlineUser;

            if (_onlineUsers.TryGetValue(userId, out onlineUser))
            {
                onlineUser = new OnlineUser(
                        onlineUser,
                        connectionId,
                        latestHeartbeat,
                        latestInteractivity);
            }
            else
            {
                onlineUser = new OnlineUser(
                        _documentSession.Load<User>(userId),
                        connectionId,
                        latestHeartbeat,
                        latestInteractivity);
            }

            UpdateCache(userId, onlineUser);
        }

        public OnlineUser GetOnlineUser(string userId)
        {
            OnlineUser onlineUser;
            _onlineUsers.TryGetValue(userId, out onlineUser);
            return onlineUser;
        }

        public OnlineUser GetOnlineUserByConnectionId(string connectionId)
        {
            return _onlineUsers
                .Where(x => x.Value.ConnectionIds.Any(y => y == connectionId))
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        private void UpdateCache(string userId, OnlineUser onlineUser)
        {
            _onlineUsers.AddOrUpdate(userId, onlineUser, (x, y) => onlineUser);

            _messageBus.Publish(onlineUser.GetUnPublishedEvents().First());
        }
    }
}
