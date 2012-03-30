/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Sessions;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Services
{
    public class HubService : IHubService
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public HubService(
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService
           )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");

            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public UserProfile GetUserProfile(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var user = _documentSession.Load<User>(userId);

            return new UserProfile()
            {
                Id = user.Id,
                Name = user.GetName(),
                LastLoggedIn = user.LastLoggedIn,
                Avatar = GetUserAvatar(user),
                Status = UserOnlineStatus(userId)
            };
        }

        public Avatar GetUserAvatar(User user)
        {
            if (user.Avatar != null)
            {
                return new Avatar()
                {
                    AltTag = user.GetName(),
                    UrlToImage = _mediaFilePathService.MakeMediaFileUri(
                        user.Avatar.Id,
                        "image",
                        "avatar",
                        user.Avatar.Metadata["metatype"]
                        )
                };
            }

            return new Avatar()
            {
                AltTag = user.GetName(),
                UrlToImage = _configService.GetDefaultAvatar("user")
            };
        }

        public IEnumerable<All_ChatSessions.Results> GetClientsForChat(string chatId)
        {
            return _documentSession
                .Query<All_ChatSessions.Results, All_ChatSessions>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .AsProjection<All_ChatSessions.Results>()
                .Where(x => x.ChatId == chatId && x.Status < (int)Connection.ConnectionStatus.Offline)
                .ToList();
        }

        public IEnumerable<All_ChatSessions.Results> GetChatsForAClient(string clientId)
        {
            return _documentSession
                .Query<All_ChatSessions.Results, All_ChatSessions>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .AsProjection<All_ChatSessions.Results>()
                .Where(x => x.ClientId == clientId)
                .ToList();
        }

        public string GetClientsUserId(string clientId)
        {
            return _documentSession
                .Query<All_UserSessions.Results, All_UserSessions>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .AsProjection<All_UserSessions.Results>()
                .Where(x => x.ClientId == clientId)
                .ToList()
                .FirstOrDefault().UserId;
        }

        public void UpdateUserOnline(string clientId, string userId)
        {
            User user;

            if (UserExists(userId, out user))
            {
                UserSession session;

                if (SessionExists(clientId, userId, out session))
                {
                    UpdateUserSession(clientId, (int)Connection.ConnectionStatus.Online);
                }
                else
                {
                    PersistUserSession(userId, clientId, (int) Connection.ConnectionStatus.Online);
                }
            }
        }
 
        public void UpdateChatUserStatus(string chatId, string clientId, string userId, int status)
        {
            if (chatId.Contains('/'))
            {
                UpdateGroupChatSessions(chatId, clientId, userId, status);
            }

            else 
            {
                UpdatePrivateChatSessions(chatId, clientId, userId, status);
            }
        }

        public string DisconnectClient(string clientId)
        {
            var userSession = _documentSession
                .Query<UserSession>()
                .Where(x => x.ClientId == clientId)
                .FirstOrDefault();

            DeleteUserSession(clientId);

            DeleteChatSessions(clientId);

            return userSession.User.Id;
        }

        public IEnumerable<string> GetConnectedClientIdsForAUser(string userId)
        {
            return _documentSession
                .Query<All_UserSessions.Results, All_UserSessions>()
                .AsProjection<All_UserSessions.Results>()
                .Where(x => x.Status < 2 && x.UserId == userId)
                .ToList()
                .Select(x => x.ClientId);
        }
        
        public IEnumerable<string> GetConnectedUserClientIds()
        {
            return _documentSession
                .Query<All_UserSessions.Results, All_UserSessions>()
                .AsProjection<All_UserSessions.Results>()
                .Where(x => x.Status < 2)
                .ToList()
                .Select(x => x.ClientId);
        }

        private int UserOnlineStatus(string userId)
        {
            var sessions = _documentSession
                .Query<All_UserSessions.Results, All_UserSessions>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .AsProjection<All_UserSessions.Results>()
                .Where(x => x.UserId == userId && x.Status < 2)
                .ToList();

            if(!sessions.IsNotNullAndHasItems())
            {
                return (int) Connection.ConnectionStatus.Offline;
            }

            return sessions
                .OrderByDescending(x => x.Status)
                .Select(x => x.Status)
                .First();
        }

        [Transaction]
        public void PersistChatMessage(string chatId, string userId, string message, string targetUserId = null)
        {
            try
            {
                if (chatId.Contains('/'))
                {
                    var commandHandler = new GroupChatMessageCreateCommandHandler(_documentSession);

                    commandHandler.Handle(MakeGroupChatMessageCreateCommand(chatId, userId, targetUserId, message, DateTime.UtcNow));
                }

                else
                {
                    var commandHandler = new PrivateChatMessageCreateCommandHandler(_documentSession);

                    commandHandler.Handle(MakePrivateChatMessageCreateCommand(chatId, userId, targetUserId, message, DateTime.UtcNow));
                }

                _documentSession.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Transaction]
        private void PersistUserSession(string userId, string clientId, int status)
        {
            try
            {
                var commandHandler = new UserSessionCreateCommandHandler(_documentSession);

                commandHandler.Handle(MakeUserSessionCreateCommand(clientId, userId, (int)Connection.ConnectionStatus.Online));

                _documentSession.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Transaction]
        private void UpdateUserSession(string clientId, int status)
        {
            try
            {
                var commandHandler = new UserSessionUpdateCommandHandler(_documentSession);

                var command = MakeUserSessionUpdateCommand(clientId, (int)Connection.ConnectionStatus.Online);

                commandHandler.Handle(command);

                _documentSession.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [Transaction]
        private void DeleteUserSession(string clientId)
        {
            try
            {
                var commandHandler = new UserSessionDeleteCommandHandler(_documentSession);

                commandHandler.Handle(MakeUserSessionDeleteCommand(clientId));

                _documentSession.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [Transaction]
        private void DeleteChatSessions(string clientId)
        {
            var chatIds = GetChatsForAClient(clientId);

            try
            {
                var privateChatDeleteCommandHandler = new PrivateChatSessionDeleteCommandHandler(_documentSession);
                var groupChatDeleteCommandHandler = new GroupChatSessionDeleteCommandHandler(_documentSession);

                foreach (var chat in chatIds)
                {
                    if(chat.ChatId.Contains('/'))
                    {
                        groupChatDeleteCommandHandler.Handle(MakeGroupChatSessionDeleteCommand(chat.ChatId, clientId));
                    }
                    else
                    {
                        privateChatDeleteCommandHandler.Handle(MakePrivateChatSessionDeleteCommand(chat.ChatId, clientId));
                    }
                }

                _documentSession.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Transaction]
        private void UpdatePrivateChatSessions(string chatId, string clientId, string userId, int status)
        {
            try
            {
                var sessions = _documentSession
                    .Query<PrivateChatSession>()
                    .Where(x =>
                        x.User.Id == userId &&
                        x.ChatId == chatId
                    )
                    .ToList();

                if (sessions.IsNotNullAndHasItems())
                {
                    var commandHandler = new PrivateChatSessionUpdateCommandHandler(_documentSession);

                    foreach (var session in sessions)
                    {
                        commandHandler.Handle(MakePrivateChatSessionUpdateCommand(chatId, userId, session.ClientId, status));
                    }
                }
                else
                {
                    var commandHandler = new PrivateChatSessionCreateCommandHandler(_documentSession);

                    commandHandler.Handle(MakePrivateChatSessionCreateCommand(userId, chatId, clientId, status));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            _documentSession.SaveChanges();
        }

        [Transaction]
        private void UpdateGroupChatSessions(string chatId, string clientId, string userId, int status)
        {
            try
            {
                var sessions = _documentSession
                    .Query<GroupChatSession>()
                    .Where(x =>
                        x.User.Id == userId &&
                        x.GroupId == chatId
                    )
                    .ToList();

                if (sessions.IsNotNullAndHasItems())
                {
                    var commandHandler = new GroupChatSessionUpdateCommandHandler(_documentSession);

                    foreach (var session in sessions)
                    {
                        commandHandler.Handle(MakeGroupChatSessionUpdateCommand(chatId, userId, session.ClientId, status));
                    }
                }
                else
                {
                    var commandHandler = new GroupChatSessionCreateCommandHandler(_documentSession);

                    commandHandler.Handle(MakeGroupChatSessionCreateCommand(chatId, userId, clientId, status));
                }

                _documentSession.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private bool UserExists(string userId, out User user)
        {
            user = _documentSession.Load<User>(userId);

            return user != null;
        }

        private bool SessionExists(string clientId, string userId, out UserSession clientSession)
        {
            clientSession = _documentSession
                .Query<UserSession>()
                .Where(x => x.ClientId == clientId && x.User.Id == userId)
                .FirstOrDefault();

            return clientSession != null;
        }

        private static PrivateChatSessionCreateCommand MakePrivateChatSessionCreateCommand(string userId, string chatId, string clientId, int status)
        {
            return new PrivateChatSessionCreateCommand()
            {
                UserId = userId,
                ChatId = chatId,
                ClientId = clientId,
                CreatedDateTime = DateTime.UtcNow,
                LatestActivity = DateTime.UtcNow,
                Status = status
            };
        }

        private static PrivateChatSessionUpdateCommand MakePrivateChatSessionUpdateCommand(string chatId, string userId, string clientId, int status)
        {
            return new PrivateChatSessionUpdateCommand()
            {
                ChatId = chatId,
                ClientId = clientId,
                LastActivity = DateTime.UtcNow,
                Status = status,
                UserId = userId
            };
        }

        private static PrivateChatSessionDeleteCommand MakePrivateChatSessionDeleteCommand(string chatId, string clientId)
        {
            return new PrivateChatSessionDeleteCommand()
            {
                ChatId = chatId,
                ClientId = clientId
            };
        }

        private static GroupChatSessionUpdateCommand MakeGroupChatSessionUpdateCommand(string chatId, string userId, string clientId, int status)
        {
            return new GroupChatSessionUpdateCommand()
            {
                GroupId = chatId,
                ClientId = clientId,
                LastActivity = DateTime.UtcNow,
                Status = status,
                UserId = userId
            };
        }

        private static GroupChatSessionCreateCommand MakeGroupChatSessionCreateCommand(string chatId, string userId, string clientId, int status)
        {
            return new GroupChatSessionCreateCommand()
            {
                GroupId = chatId,
                UserId = userId,
                ClientId = clientId,
                LastActivity = DateTime.UtcNow,
                Status = status
            };
        }

        private static GroupChatSessionDeleteCommand MakeGroupChatSessionDeleteCommand(string chatId, string clientId)
        {
            return new GroupChatSessionDeleteCommand()
            {
                ClientId = clientId,
                GroupId = chatId
            };
        }

        private static GroupChatMessageCreateCommand MakeGroupChatMessageCreateCommand(string groupId, string userId, string targetUserId, string message, DateTime timestamp)
        {
            return new GroupChatMessageCreateCommand()
            {
                GroupId = groupId,
                Message = message,
                Timestamp = timestamp,
                UserId = userId,
                TargetUserId = targetUserId
            };
        }

        private static PrivateChatMessageCreateCommand MakePrivateChatMessageCreateCommand(string chatId, string userId, string targetUserId, string message, DateTime timestamp)
        {
            return new PrivateChatMessageCreateCommand()
            {
                ChatId = chatId,
                Message = message,
                Timestamp = timestamp,
                UserId = userId,
                TargetUserId = targetUserId
            };
        }

        private static UserSessionCreateCommand MakeUserSessionCreateCommand(string clientId, string userId, int status)
        {
            return new UserSessionCreateCommand()
            {
                ClientId = clientId,
                CreatedDateTime = DateTime.UtcNow,
                UserId = userId,
                LatestActivity = DateTime.UtcNow,
                Status = status
            };
        }

        private static UserSessionUpdateCommand MakeUserSessionUpdateCommand(string clientId, int status)
        {
            return new UserSessionUpdateCommand()
            {
                ClientId = clientId,
                LatestActivity = DateTime.UtcNow,
                Status = status
            };
        }

        private static UserSessionDeleteCommand MakeUserSessionDeleteCommand(string clientId)
        {
            return new UserSessionDeleteCommand()
            {
                ClientId = clientId
            };
        }

        public string GetGroupName(string chatId)
        {
            var group = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Id == chatId)
                .FirstOrDefault();

            return group != null ? group.Name : "Unknown Group";
        }

        public List<ChatMessage> GetChatMessages(string chatId)
        {
            return _documentSession
                .Query<ChatMessage>()
                .Include(x => x.User.Id)
                .Where(x => x.ChatId == chatId)
                .OrderByDescending(x => x.Timestamp)
                .Take(10)
                .ToList()
                .Select(x => new ChatMessage()
                                 {
                                     Id = x.Id,
                                     Message = x.Message,
                                     Timestamp = x.Timestamp
                                 })
                .ToList();
        }

        #endregion
    }
}