using System;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Config
{
    public interface IOnlineUserCache
    {
        void AddUserSession(string userId, string connectionId);

        IEnumerable<OnlineUser> CurrentlyOnlineUsers(DateTime since);

        void UpdateUserSession(string userId, string connectionId, DateTime latestHeartbeat, DateTime latestInteractivity);

        OnlineUser GetOnlineUser(string userId);

        OnlineUser GetOnlineUserByConnectionId(string connectionId);
    }
}
