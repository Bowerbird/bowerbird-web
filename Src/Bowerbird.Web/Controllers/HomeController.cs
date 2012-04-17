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
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Services;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.Paging;

namespace Bowerbird.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;
        private readonly IDocumentStore _documentStore;
        private readonly IObservationViewFactory _observationViewFactory;
        private readonly IBrowseItemFactory _browseItemFactory;
        private readonly IStreamItemFactory _streamItemFactory;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService,
            IDocumentStore documentStore,
            IObservationViewFactory observationViewFactory,
            IBrowseItemFactory browseItemFactory,
            IStreamItemFactory streamItemFactory
        )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");
            Check.RequireNotNull(documentStore, "documentStore");
            Check.RequireNotNull(observationViewFactory, "observationViewFactory");
            Check.RequireNotNull(browseItemFactory, "browseItemFactory");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
            _documentStore = documentStore;
            _observationViewFactory = observationViewFactory;
            _browseItemFactory = browseItemFactory;
            _streamItemFactory = streamItemFactory;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (Request.IsAjaxRequest())
                {
                    return new JsonNetResult(MakeHomeIndex());
                }

                ViewBag.HomeIndex = MakeHomeIndex();

                return View();
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        public ActionResult SetupTestData()
        {
            _commandProcessor.Process(new SetupTestDataCommand());

            return RedirectToAction("index");
        }

        private HomeIndex MakeHomeIndex()
        {
            var memberships = _documentSession
                .Query<Member>()
                .Include(x => x.User.Id)
                .Include(x => x.Group.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .ToList();

            var teams = memberships
                .Where(x => x.Group.Id.Contains("teams/"))
                .Select(x =>
                        new TeamView
                            {
                                Id = x.Group.Id,
                                Name = x.Group.Name,
                                Avatar =
                                    new Avatar() { UrlToImage = "/img/default-team-avatar.jpg", AltTag = x.Group.Name }
                            }
                )
                .ToList();

            var projects = memberships
                .Where(x => x.Group.Id.Contains("projects/"))
                .Select(x =>
                        new ProjectView
                            {
                                Id = x.Group.Id,
                                Name = x.Group.Name,
                                Avatar =
                                    new Avatar() { UrlToImage = "/img/default-project-avatar.jpg", AltTag = x.Group.Name }
                            }
                )
                .ToList();

            var projectMenu = memberships
                .Where(x => x.Group.Id.Contains("projects/"))
                .Select(x =>
                        new MenuItem()
                            {
                                Id = x.Group.Id,
                                Name = x.Group.Name
                            }
                )
                .ToList();

            var teamMenu = memberships
                .Where(x => x.Group.Id.Contains("teams/"))
                .Select(x =>
                        new MenuItem()
                            {
                                Id = x.Group.Id,
                                Name = x.Group.Name
                            }
                )
                .ToList();

            var watchlistMenu = _documentSession
                .Query<Watchlist>()
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x =>
                        new MenuItem()
                            {
                                Id = x.QuerystringJson,
                                Name = x.Name
                            })
                .ToList();

            var userProfile = GetUserProfile(_userContext.GetAuthenticatedUserId());

            var onlineUsers = GetCurrentlyOnlineUsers();

            var homeIndex = new HomeIndex()
            {
                ProjectMenu = projectMenu,
                TeamMenu = teamMenu,
                WatchlistMenu = watchlistMenu,
                Projects = projects,
                Teams = teams,
                UserProfile = userProfile,
                OnlineUsers = onlineUsers,
                TeamsJson = JsonConvert.SerializeObject(teams),
                ProjectsJson = JsonConvert.SerializeObject(projects),
                OnlineUsersJson = JsonConvert.SerializeObject(onlineUsers),
                UserProfileJson = JsonConvert.SerializeObject(userProfile)
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
                        user.Avatar.Metadata["metatype"])
                };
            }
            else
            {
                return new Avatar()
                {
                    AltTag = user.GetName(),
                    UrlToImage = AvatarUris.DefaultUser
                };
            }
        }

        #endregion
    }
}