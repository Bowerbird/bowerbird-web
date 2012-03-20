/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.DomainModels.Sessions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService
        )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(HomeIndexInput homeIndexInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(MakeHomeIndex(homeIndexInput));
            }
            return View(MakeHomeIndex(homeIndexInput));

            //return View(new HomeIndex(){UserProfile = new UserProfile(){Id = User.Identity.Name}});
        }

        [HttpGet]
        public ActionResult ActivityTest()
        {
            return View();
        }

        private HomeIndex MakeHomeIndex(HomeIndexInput indexInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .ToList();

            var user = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            var homeIndex = new HomeIndex()
            {
                ProjectMenu = groupMemberships
                .Where(x => x.Group.Id.Contains("projects/"))
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name
                    }
                )
                .ToList(),

                Projects = groupMemberships
                .Where(x => x.Group.Id.Contains("projects/"))
                .Select(x =>
                    new ProjectView
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name,
                        Avatar = new Avatar() { UrlToImage = "/images/default-project-avatar.png", AltTag = x.Group.Name }
                    }
                )
                .ToList(),

                TeamMenu = groupMemberships
                .Where(x => x.Group.Id.Contains("teams/"))
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name
                    }
                )
                .ToList(),

                Teams = groupMemberships
                .Where(x => x.Group.Id.Contains("teams/"))
                .Select(x =>
                    new TeamView
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name,
                        Avatar = new Avatar() { UrlToImage = "/images/default-team-avatar.png", AltTag = x.Group.Name }
                    }
                )
                .ToList(),

                UserProfile = GetUserProfile(_userContext.GetAuthenticatedUserId()),

                OnlineUsers = GetCurrentlyOnlineUsers(),

                WatchlistMenu = _documentSession
                .Query<Watchlist>()
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.QuerystringJson,
                        Name = x.Name
                    })
                    .ToList()
            };

            return homeIndex;
        }

        private IEnumerable<UserProfile> GetCurrentlyOnlineUsers()
        {
            var connectedUsers = _documentSession.Query<User>()
                .Where(x => x.Id.In(GetConnectedUserIds()))
                .ToList();

            return connectedUsers
                .Select(x => GetUserProfile(x.Id))
                .ToList();
        }

        private UserProfile GetUserProfile(string userId)
        {
            var user = _documentSession.Load<User>(userId);

            return new UserProfile()
            {
                Id = user.Id,
                Name = user.GetName(),
                LastLoggedIn = user.LastLoggedIn,
                Avatar = GetUserAvatar(user)
            };
        }

        private IEnumerable<string> GetConnectedUserIds()
        {
            return _documentSession
                .Query<All_UserSessions.Results, All_UserSessions>()
                .AsProjection<All_UserSessions.Results>()
                .Where(x => x.Status < (int)Connection.ConnectionStatus.Offline)
                .ToList()
                .Select(x => x.UserId)
                .Distinct();
        }

        private Avatar GetUserAvatar(User user)
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
            else
            {
                return new Avatar()
                {
                    AltTag = user.GetName(),
                    UrlToImage = _configService.GetDefaultAvatar("user")
                };
            }
        }

        #endregion
    }
}