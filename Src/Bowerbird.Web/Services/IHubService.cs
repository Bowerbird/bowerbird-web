using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.ViewModels;

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

        string DisconnectClient(string clientId);

        IEnumerable<string> GetConnectedUserClientIds();

        string GetClientsUserId(string clientId);

        IEnumerable<string> GetConnectedClientIdsForAUser(string userId);

        string GetGroupName(string chatId);

        List<ChatMessage> GetChatMessages(string chatId);
    }
}