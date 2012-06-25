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
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Collections;
using Bowerbird.Core.Config;
using Bowerbird.Core.Factories;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.Builders
{
    public class UserViewModelBuilder : IUserViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserContext _userContext;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public UserViewModelBuilder(
            IDocumentSession documentSession,
            IUserContext userContext,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _documentSession = documentSession;
            _userContext = userContext;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildAuthenticatedUser()
        {
            //var groups = _documentSession.Query<All_Groups.Result, All_Groups>()
            //        .Where(x => x.UserIds.Any(y => y == userId))
            //        .Include(x => userId)
            //        .AsProjection<All_Groups.Result>()
            //        .ToList();

            //var user = _documentSession.Load<User>(userId);
            //var application = groups.Any(x => x.GroupType == "approot") ? groups.Single(x => x.GroupType == "approot").AppRoot : (AppRoot)null;
            //var organisations = groups.Where(x => x.GroupType == "organisation").Select(MakeOrganisation);
            //var teams = groups.Where(x => x.GroupType == "team").Select(MakeTeam);
            //var projects = groups.Where(x => x.GroupType == "project").Select(MakeProject);
            //var memberships = groups.SelectMany(x => x.Users.Where(y => y.Id == userId));

            var user = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            var groupResults = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.UserIds.Any(y => y == user.Id))
                .ToList();

            return new
            {
                User = _userViewFactory.Make(user),
                AppRoot = groupResults.Where(x => x.Group is AppRoot).Select(x => x.Group as AppRoot).First(),
                Organisations = groupResults.Where(x => x.Group is Organisation).Select(x => _groupViewFactory.Make(x.Group)),
                Teams = groupResults.Where(x => x.Group is Team).Select(x => _groupViewFactory.Make(x.Team)),
                Projects = groupResults.Where(x => x.Group is Project).Select(x => _groupViewFactory.Make(x.Project)),
                UserProjects = groupResults.Where(x => x.Group is UserProject).Select(x => _groupViewFactory.Make(x.UserProject)),
                Memberships = user.Memberships.Select(x => new {
                    GroupId = x.Group.Id,
                    x.Group.GroupType,
                    RoleIds = x.Roles.Select(y => y.Id),
                    PermissionIds = x.Roles.SelectMany(y => y.Permissions).Select(y => y.Id)
                })
            };
        }

        public object BuildUser(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _userViewFactory.Make(_documentSession.Load<User>(idInput.Id));
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
                .Select(_userViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        ///// <summary>
        ///// PagingInput.Id is User.Id where User is User being Followed
        ///// </summary>
        //public object BuildUsersFollowingList(PagingInput pagingInput)
        //{
        //    RavenQueryStatistics stats;

        //    return _documentSession
        //        .Query<FollowUser>()
        //        .Where(x => x.UserToFollow.Id == pagingInput.Id)
        //        .Include(x => x.UserToFollow.Id)
        //        .Statistics(out stats)
        //        .Skip(pagingInput.Page)
        //        .Take(pagingInput.PageSize)
        //        .ToList()
        //        .Select(x => MakeUser(_documentSession.Load<User>(x.Id)))
        //        .ToPagedList(
        //            pagingInput.Page,
        //            pagingInput.PageSize,
        //            stats.TotalResults,
        //            null);
        //}

        ///// <summary>
        ///// PagingInput.Id is User.Id where User is the User following other
        ///// </summary>
        //public object BuildUsersBeingFollowedByList(PagingInput pagingInput)
        //{
        //    RavenQueryStatistics stats;

        //    return _documentSession
        //        .Query<FollowUser>()
        //        .Where(x => x.Follower.Id == pagingInput.Id)
        //        .Include(x => x.Follower.Id)
        //        .Statistics(out stats)
        //        .Skip(pagingInput.Page)
        //        .Take(pagingInput.PageSize)
        //        .ToList()
        //        .Select(x => MakeUser(_documentSession.Load<User>(x.Id)))
        //        .ToPagedList(
        //            pagingInput.Page,
        //            pagingInput.PageSize,
        //            stats.TotalResults,
        //            null);
        //}

        public object BuildOnlineUsers()
        {
            // Return connected users (those users active less than 5 minutes ago)
            var fiveMinutesAgo = DateTime.UtcNow - TimeSpan.FromMinutes(5);

            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.LatestActivity.Any(y => y > fiveMinutesAgo))
                .ToList()
                .Select(x => _userViewFactory.Make(x.User));
        }

        private object MakeMember(Member member)
        {
            return new
            {
                GroupId = member.Group.Id,
                Roles = from role in member.Roles
                        select new
                        {
                            Id = role.ShortId(),
                            Permissions = role.Permissions.Select(x => x.ShortId())
                        }
            };
        }

        //private object MakeProject(All_Groups.Result result)
        //{
        //    return new
        //    {
        //        Id = result.Project.Id,
        //        result.Project.Name,
        //        result.Project.Description,
        //        result.Project.Website,
        //        Avatar = result.Project.Avatar,
        //        MemberCount = result.UserIds.Count()
        //    };
        //}

        //private object MakeTeam(All_Groups.Result result)
        //{
        //    return new
        //    {
        //        Id = result.Team.Id,
        //        result.Team.Name,
        //        result.Team.Description,
        //        result.Team.Website,
        //        Avatar = result.Team.Avatar,
        //        MemberCount = result.UserIds.Count()
        //    };
        //}

        //private object MakeOrganisation(All_Groups.Result result)
        //{
        //    return new
        //    {
        //        Id = result.Organisation.Id,
        //        result.Organisation.Name,
        //        result.Organisation.Description,
        //        result.Organisation.Website,
        //        Avatar = result.Organisation.Avatar,
        //        MemberCount = result.UserIds.Count()
        //    };
        //}

        #endregion
    }
}