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
            return BuildUser(new IdInput() { Id = _userContext.GetAuthenticatedUserId() });
        }

        public object BuildUser(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return MakeUser(idInput.Id);
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
                .Select(x => MakeUser(x.Id))
               .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null);
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
                .Where(x => x.Status < (int)Connection.ConnectionStatus.Offline)
                .ToList()
                .Select(x => x.UserId)
                .Distinct();

            var connectedUsers = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.User.Id.In(connectedUserIds))
                .ToList();

            return connectedUsers
                .Select(MakeUser)
                .ToList();
        }

        private object MakeUser(string userId)
        {
            return MakeUser(_documentSession.Load<User>(userId));
        }

        private object MakeUser(User user)
        {
            return new
            {
                Avatar = _avatarFactory.Make(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        private object MakeUser(All_Users.Result user)
        {
            Check.RequireNotNull(user, "user");

            // grab the user's groups
            var userGroups = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Group.Id.In(user.Groups.SelectMany(y => y.Id)))
                .ToList();

            // make and return the user
            return new
            {
                Avatar = _avatarFactory.Make(user.User),
                user.User.Id,
                user.User.LastLoggedIn,
                Name = user.User.GetName(),
                Projects = userGroups.Where(x => x.GroupType == "project").Select(MakeProject),
                Teams = userGroups.Where(x => x.GroupType == "team").Select(MakeTeam)
            };
        }

        private object MakeProject(All_Groups.Result project)
        {
            return new
            {
                project.Id,
                project.Project.Name,
                project.Project.Description,
                project.Project.Website,
                Avatar = _avatarFactory.Make(project.Project),
                project.GroupMemberCount
            };
        }

        public object MakeTeam(All_Groups.Result team)
        {
            return new
            {
                team.Id,
                team.Team.Name,
                team.Team.Description,
                team.Team.Website,
                Avatar = _avatarFactory.Make(team.Team),
                team.GroupMemberCount,
                Projects = 0
            };
        }

        #endregion
    }
}