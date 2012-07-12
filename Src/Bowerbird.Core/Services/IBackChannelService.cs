/* Bowerbird V1 - Licensed under MIT 1.1 Public License

Developers:
* Frank Radocaj : frank@radocaj.com
* Hamish Crittenden : hamish.crittenden@gmail.com
Project Manager:
* Ken Walker : kwalker@museum.vic.gov.au
Funded by:
* Atlas of Living Australia
*/

using System.Collections;
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.VideoUtilities;

namespace Bowerbird.Core.VideoUtilities
{
    public interface IBackChannelService : IService
    {
        #region User Channel

        /// <summary>
        /// Adds user connection to user's own channel
        /// </summary>
        void AddUserToUserChannel(string userId, string connectionId);

        /// <summary>
        /// Send online user list to user's own channel
        /// </summary>
        void SendOnlineUsersToUserChannel(string userId, object onlineUsers);

        /// <summary>
        /// Send group joining acknowledgement to user's own channel
        /// </summary>
        void SendJoinedGroupToUserChannel(string userId, object group);

        /// <summary>
        /// Send chat joined acknowledgement to user's own channel
        /// </summary>
        void NotifyChatJoinedToUserChannel(string userId, object chatDetails);

        /// <summary>
        /// Send chat existed acknowledgement to user's own channel
        /// </summary>
        void NotifyChatExitedToUserChannel(string userId, string chatId);

        /// <summary>
        /// Pass a newly created media resource object back to the user
        /// </summary>
        void SendUploadedMediaResourceToUserChannel(string userId, object mediaResource);

        #endregion

        #region Group Channel

        /// <summary>
        /// Add user connection to group channel
        /// </summary>
        void AddUserToGroupChannel(string groupId, string connectionId);

        /// <summary>
        /// Send new activity to group channel
        /// </summary>
        void SendActivityToGroupChannel(dynamic activity);

        #endregion

        #region Online Users Channel

        /// <summary>
        /// Add user connection to online users channel
        /// </summary>
        void AddUserToOnlineUsersChannel(string connectionId);

        /// <summary>
        /// Send user status update to online users channel
        /// </summary>
        void SendUserStatusUpdateToOnlineUsersChannel(object userStatus);

        #endregion

        #region Chat Channel

        /// <summary>
        /// Add user connection to chat channel
        /// </summary>
        void AddUserToChatChannel(string chatId, string connectionId);

        /// <summary>
        /// Remove user connection from chat channel
        /// </summary>
        void RemoveUserFromChatChannel(string chatId, string connectionId);

        /// <summary>
        /// Send new message to chat channel
        /// </summary>
        void NewChatMessageToChatChannel(string chatId, object chatMessageDetails);

        /// <summary>
        /// Send user joined acknowledgement to chat channel
        /// </summary>
        void UserJoinedChatToChatChannel(string chatId, object chatMessageDetails);

        /// <summary>
        /// Send user exited acknowledgement to chat channel
        /// </summary>
        void UserExitedChatToChatChannel(string chatId, object chatMessageDetails);

        #endregion

        #region Debug Channel

        void DebugToClient(object output);

        #endregion
    }
}