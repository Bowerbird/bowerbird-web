using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.ViewModels.Shared;

namespace Bowerbird.Web.Services
{
    public interface IHubService
    {
        UserProfile GetUserProfile(string userId);

        Avatar GetUserAvatar(User user);

        IEnumerable<All_ChatSessions.Results> GetClientsForChat(string chatId);
        
        void UpdateUserOnline(string clientId, string userId);
        
        void PersistChatMessage(string chatId, string userId, string targetUserId, string message);
        
        void UpdateChatUserStatus(string chatId, string clientId, string userId, int status);

        bool DisconnectClient(string clientId, out string userId);

        IEnumerable<string> GetConnectedUserClientIds();

        string GetClientsUserId(string clientId);
    }
}