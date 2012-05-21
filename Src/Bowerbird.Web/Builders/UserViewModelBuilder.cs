/* Bowerbird V1 - Licensed under MIT 1.1 Public License

Developers:
* Frank Radocaj : frank@radocaj.com
* Hamish Crittenden : hamish.crittenden@gmail.com
Project Manager:
* Ken Walker : kwalker@museum.vic.gov.au
Funded by:
* Atlas of Living Australia
*/

using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Collections;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Builders
{
    public class UserViewModelBuilder : IUserViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserContext _userContext;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public UserViewModelBuilder(
            IDocumentSession documentSession,
            IUserContext userContext,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _userContext = userContext;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildAuthenticatedUser()
        {
            var userId = _userContext.GetAuthenticatedUserId();

            var projects = _documentSession.Query<All_Groups.Result, All_Groups>()
                    .Where(x => x.UserIds.Any(y => y == userId) && x.GroupType == "project")
                    .Include(x => userId)
                    .AsProjection<All_Groups.Result>()
                    .ToList()
                    .Select(MakeProject);

            var user = _documentSession.Load<User>(userId);

            return new
            {
                User = new
                {
                    user.Id,
                    Avatar = _avatarFactory.Make(user),
                    user.LastLoggedIn,
                    Name = user.GetName()
                },
                Projects = projects
            };
        }

        public object BuildUser(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var user = _documentSession.Load<User>(idInput.Id);

            return MakeUser(user);
        }

        public object BuildUserList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<User>()
                .Include(x => x.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeUser)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is User.Id where User is User being Followed
        /// </summary>
        public object BuildUsersFollowingList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<FollowUser>()
                .Where(x => x.UserToFollow.Id == pagingInput.Id)
                .Include(x => x.UserToFollow.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeUser(_documentSession.Load<User>(x.Id)))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        /// <summary>
        /// PagingInput.Id is User.Id where User is the User following other
        /// </summary>
        public object BuildUsersBeingFollowedByList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<FollowUser>()
                .Where(x => x.Follower.Id == pagingInput.Id)
                .Include(x => x.Follower.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeUser(_documentSession.Load<User>(x.Id)))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
        }

        public IEnumerable BuildOnlineUsers()
        {
            var connectedUserIds = _documentSession
                .Query<All_Sessions.Results, All_Sessions>()
                .AsProjection<All_Sessions.Results>()
                .Include(x => x.UserId)
                .Where(x => x.Status < (int) Connection.ConnectionStatus.Offline)
                .ToList()
                .Select(x => x.UserId)
                .Distinct();

            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .Where(x => x.UserId.In(connectedUserIds))
                .AsProjection<All_Users.Result>()
                .ToList()
                .Select(x => MakeUser(x.User));
        }

        //private object MakeUser(string userId)
        //{
        //    return MakeUser(_documentSession.Load<User>(userId));
        //}

        private object MakeUser(User user)
        {
            return new
            {
                user.Id,
                Avatar = _avatarFactory.Make(user),
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        ////TODO: Change this method to query the All_Groups index.
        //private object MakeUser(All_Users.ClientResult user)
        //{
        //    Check.RequireNotNull(user, "user");

        //    // grab the user's groups
        //    var userGroups = _documentSession
        //        .Query<All_Groups.Result, All_Groups>()
        //        .AsProjection<All_Groups.ClientResult>()
        //        .Where(x => x.Group.Id.In(user.Memberships.Select(y => y.Id)))
        //        .ToList();

        //    // make and return the user
        //    return new
        //    {
        //        Avatar = _avatarFactory.Make(user.User),
        //        user.User.Id,
        //        user.User.LastLoggedIn,
        //        Name = user.User.GetName()
        //        //Projects = userGroups.Where(x => x.GroupType == "project").Select(MakeProject),
        //        //Teams = userGroups.Where(x => x.GroupType == "team").Select(MakeTeam)
        //    };
        //}

        private object MakeProject(All_Groups.Result result)
        {
            return new
            {
                Id = result.Project.Id,
                result.Project.Name,
                result.Project.Description,
                result.Project.Website,
                Avatar = _avatarFactory.Make(result.Project),
                MemberCount = result.MemberIds.Count()
            };
        }

        //public object MakeTeam(All_Groups.ClientResult team)
        //{
        //    return new
        //    {
        //        Id = team.GroupId,
        //        team.Team.Name,
        //        team.Team.Description,
        //        team.Team.Website,
        //        Avatar = _avatarFactory.Make(team.Team),
        //        Memberships = team.Memberships.Count(),
        //        Projects = 0
        //    };
        //}

        #endregion
    }
}