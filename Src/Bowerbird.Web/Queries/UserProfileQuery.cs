///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Collections.Generic;
//using Bowerbird.Core.Config;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.Indexes;
//using Bowerbird.Web.ViewModels;
//using Newtonsoft.Json;
//using Raven.Client;
//using Raven.Client.Linq;
//using System.Linq;
//using Bowerbird.Web.Factories;

//namespace Bowerbird.Web.Queries
//{
//    public class UserProfileQuery : Bowerbird.Web.Queries.IUserProfileQuery
//    {
//        #region Fields

//        private readonly IUserContext _userContext;
//        private readonly IDocumentSession _documentSession;
//        private readonly IAvatarFactory _avatarFactory;
//        private readonly IProjectViewFactory _projectViewFactory;
//        private readonly ITeamViewFactory _teamViewFactory;

//        #endregion

//        #region Constructors

//        public UserProfileQuery(
//            IUserContext userContext,
//            IDocumentSession documentSession,
//            IAvatarFactory avatarFactory,
//            IProjectViewFactory projectViewFactory,
//            ITeamViewFactory teamViewFactory
//            )
//        {
//            Check.RequireNotNull(userContext, "userContext");
//            Check.RequireNotNull(documentSession, "documentSession");
//            Check.RequireNotNull(avatarFactory, "avatarFactory");
//            Check.RequireNotNull(projectViewFactory, "projectViewFactory");
//            Check.RequireNotNull(teamViewFactory, "teamViewFactory");

//            _userContext = userContext;
//            _documentSession = documentSession;
//            _avatarFactory = avatarFactory;
//            _projectViewFactory = projectViewFactory;
//            _teamViewFactory = teamViewFactory;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public UserProfile GetAuthenticatedUserProfile()
//        {
//            return GetUserProfile(_userContext.GetAuthenticatedUserId());
//        }

//        public UserProfile GetUserProfile(string userId)
//        {
//            var user = _documentSession.Load<User>(userId);

//            var memberships = _documentSession
//                .Query<Member>()
//                .Include(x => x.User.Id)
//                .Include(x => x.Group.Id)
//                .Where(x => x.User.Id == userId)
//                .ToList();

//            return MakeUserProfile(user, memberships);
//        }

//        public IEnumerable<UserProfile> GetOnlineUserProfiles()
//        {
//            var connectedUserIds = _documentSession
//                .Query<All_UserSessions.Results, All_UserSessions>()
//                .AsProjection<All_UserSessions.Results>()
//                .Include(x => x.UserId)
//                .Where(x => x.Status < (int)Connection.ConnectionStatus.Offline)
//                .ToList()
//                .Select(x => x.UserId)
//                .Distinct();

//            var connectedUsers = _documentSession.Query<User>()
//                .Where(x => x.Id.In(connectedUserIds))
//                .ToList();

//            return connectedUsers
//                .Select(x => GetUserProfile(x.Id))
//                .ToList();
//        }

//        private UserProfile MakeUserProfile(User user, IEnumerable<Member> memberships)
//        {
//            return new UserProfile()
//            {
//                Id = user.Id,
//                Name = user.GetName(),
//                LastLoggedIn = user.LastLoggedIn,
//                Avatar = _avatarFactory.Make(user),
//                Projects = _documentSession.Load<Project>(memberships.Where(x => x.Group.Id.StartsWith("projects/")).Select(x => x.Group.Id)).Select(x => _projectViewFactory.Make(x)),
//                Teams = _documentSession.Load<Team>(memberships.Where(x => x.Group.Id.StartsWith("teams/")).Select(x => x.Group.Id)).Select(x => _teamViewFactory.Make(x))
//            };
//        }

//        //private ClientUserContext MakeClientUserContext()
//        //{
//        //    //var memberships = _documentSession
//        //    //    .Query<Member>()
//        //    //    .Include(x => x.User.Id)
//        //    //    .Include(x => x.Group.Id)
//        //    //    .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
//        //    //    .ToList();

//        //    //var teams = memberships
//        //    //    .Where(x => x.Group.Id.Contains("teams/"))
//        //    //    .Select(x =>
//        //    //            new TeamView
//        //    //            {
//        //    //                Id = x.Group.Id,
//        //    //                Name = x.Group.Name,
//        //    //                Avatar = _avatarFactory.GetAvatar(_documentSession.Load<Team>(x.Group.Id))
//        //    //            }
//        //    //    )
//        //    //    .ToList();

//        //    //var projects = memberships
//        //    //    .Where(x => x.Group.Id.Contains("projects/"))
//        //    //    .Select(x =>
//        //    //            new ProjectView
//        //    //            {
//        //    //                Id = x.Group.Id,
//        //    //                Name = x.Group.Name,
//        //    //                Avatar = _avatarFactory.GetAvatar(_documentSession.Load<Project>(x.Group.Id))
//        //    //            }
//        //    //    )
//        //    //    .ToList();

//        //    //var projectMenu = memberships
//        //    //    .Where(x => x.Group.Id.Contains("projects/"))
//        //    //    .Select(x =>
//        //    //            new MenuItem()
//        //    //            {
//        //    //                Id = x.Group.Id,
//        //    //                Name = x.Group.Name
//        //    //            }
//        //    //    )
//        //    //    .ToList();

//        //    //var teamMenu = memberships
//        //    //    .Where(x => x.Group.Id.Contains("teams/"))
//        //    //    .Select(x =>
//        //    //            new MenuItem()
//        //    //            {
//        //    //                Id = x.Group.Id,
//        //    //                Name = x.Group.Name
//        //    //            }
//        //    //    )
//        //    //    .ToList();

//        //    //var watchlistMenu = _documentSession
//        //    //    .Query<Watchlist>()
//        //    //    .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
//        //    //    .Select(x =>
//        //    //            new MenuItem()
//        //    //            {
//        //    //                Id = x.QuerystringJson,
//        //    //                Name = x.Name
//        //    //            })
//        //    //    .ToList();

//        //    var userProfile = GetUserProfile(_userContext.GetAuthenticatedUserId());

//        //    //var onlineUsers = GetCurrentlyOnlineUsers();

//        //    return new ClientUserContext()
//        //    {
//        //        //ProjectMenu = projectMenu,
//        //        //TeamMenu = teamMenu,
//        //        //WatchlistMenu = watchlistMenu,
//        //        //Projects = projects,
//        //        //Teams = teams,
//        //        UserProfile = userProfile,
//        //        //OnlineUsers = onlineUsers,
//        //        //TeamsJson = JsonConvert.SerializeObject(teams),
//        //        //ProjectsJson = JsonConvert.SerializeObject(projects),
//        //        //OnlineUsersJson = JsonConvert.SerializeObject(onlineUsers),
//        //        //UserProfileJson = JsonConvert.SerializeObject(userProfile)
//        //    };
//        //}

//        //private IEnumerable<UserProfile> GetCurrentlyOnlineUsers()
//        //{
//        //    var connectedUsers = _documentSession.Query<User>()
//        //        .Where(x => x.Id.In(GetConnectedUserIds()))
//        //        .ToList();

//        //    return connectedUsers
//        //        .Select(x => GetUserProfile(x.Id))
//        //        .ToList();
//        //}

//        //private IEnumerable<string> GetConnectedUserIds()
//        //{
//        //    return _documentSession
//        //        .Query<All_UserSessions.Results, All_UserSessions>()
//        //        .AsProjection<All_UserSessions.Results>()
//        //        .Where(x => x.Status < (int)Connection.ConnectionStatus.Offline)
//        //        .ToList()
//        //        .Select(x => x.UserId)
//        //        .Distinct();
//        //}

//        #endregion
//    }
//}