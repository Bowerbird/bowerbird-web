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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using NodaTime;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.Builders
{
    public class UserViewModelBuilder : IUserViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public UserViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildUser(string userId)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");

            return _userViewFactory.Make(_documentSession.Load<User>(userId));
        }

        public object BuildUpdateUser(string userId)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");

            var user = _documentSession.Load<User>(userId);

            return new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Description,
                AvatarId = user.Avatar.Id,
                user.DefaultLicence,
                user.Avatar,
                user.Timezone
            };
        }

        public object BuildAuthenticatedUser(string userId)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");

            var user = _documentSession.Load<User>(userId);

            var groupResults = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.UserIds.Any(y => y == userId))
                .ToList();

            return new
            {
                User = _userViewFactory.Make(user),
                AppRoot = groupResults.Where(x => x.Group is AppRoot).Select(x => x.Group as AppRoot).First(),
                Organisations = groupResults.Where(x => x.Group is Organisation).Select(x => _groupViewFactory.Make(x.Group, user)),
                Teams = groupResults.Where(x => x.Group is Team).Select(x => _groupViewFactory.Make(x.Group, user)),
                Projects = groupResults.Where(x => x.Group is Project).Select(x => _groupViewFactory.Make(x.Group, user)),
                UserProjects = groupResults.Where(x => x.Group is UserProject).Select(x => _groupViewFactory.Make(x.Group, user)),
                Memberships = user.Memberships.Select(x => new {
                    GroupId = x.Group.Id,
                    x.Group.GroupType,
                    RoleIds = x.Roles.Select(y => y.Id),
                    PermissionIds = x.Roles.SelectMany(y => y.Permissions).Select(y => y.Id)
                }),
                user.DefaultLicence,
                TimezoneOffset = DateTimeZoneProviders.Tzdb[user.Timezone].GetOffsetFromUtc(new Instant()).ToString(),
                user.CallsToAction
            };
        }

        public object BuildOnlineUserList()
        {
            // Return connected users (those users active less than 5 minutes ago)
            var fiveMinutesAgo = DateTime.UtcNow - TimeSpan.FromMinutes(5);

            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.LatestHeartbeat.Any(y => y > fiveMinutesAgo))
                .Take(100) //HACK: Need to work out how we will list more than RavenDB max
                .ToList()
                .Select(x => _userViewFactory.Make(x.User));
        }

        public object BuildGroupUserList(string groupId, string role)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");

            RavenQueryStatistics stats;

            return _documentSession
                .Advanced.LuceneQuery<All_Users.Result, All_Users>()
                .SelectFields<All_Users.Result>("UserId", "GroupIds", "ConnectionIds", "SightingCount", "LatestHeartbeat", "LatestActivity")
                .WhereEquals(role.Replace("roles/", ""), groupId)
                .OrderBy(x => x.Name)
                .Statistics(out stats)
                .Take(100)
                .ToList()
                .Select(x => _userViewFactory.Make(x.User, true, x.SightingCount))
                .ToPagedList(
                    1,
                    100,
                    stats.TotalResults);
        }

        public object BuildGroupUserList(string groupId, UsersQueryInput usersQueryInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(usersQueryInput, "usersQueryInput");

            return ExecuteQuery(usersQueryInput, new[] { groupId });
        }

        public object BuildUserList(UsersQueryInput usersQueryInput)
        {
            Check.RequireNotNull(usersQueryInput, "usersQueryInput");

            return ExecuteQuery(usersQueryInput, new string[] { });
        }

        private object ExecuteQuery(UsersQueryInput usersQueryInput, IEnumerable<string> groupIds)
        {
            RavenQueryStatistics stats;
            //User authenticatedUser = null;

            //if (_userContext.IsUserAuthenticated())
            //{
            //    authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            //}

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Users.Result, All_Users>()
                .Statistics(out stats)
                .SelectFields<All_Users.Result>("UserId", "GroupIds", "ConnectionIds", "SightingCount", "LatestHeartbeat", "LatestActivity", "User");

            bool criteriaAdded = false;

            if (groupIds.Any())
            {
                query = query
                    .WhereIn("GroupIds", groupIds);

                criteriaAdded = true;
            }

            if (!string.IsNullOrWhiteSpace(usersQueryInput.Query))
            {
                var field = "AllFields";

                if (usersQueryInput.Field.ToLower() == "name")
                {
                    field = "Name";
                }
                if (usersQueryInput.Field.ToLower() == "description")
                {
                    field = "Description";
                }

                if (criteriaAdded)
                {
                    query = query.AndAlso();
                }

                query = query
                    .Search(field, usersQueryInput.Query);
            }

            switch (usersQueryInput.Sort.ToLower())
            {
                default:
                case "a-z":
                    query = query.AddOrder(x => x.Name, false);
                    break;
                case "z-a":
                    query = query.AddOrder(x => x.Name, true);
                    break;
            }

            return query
                .Skip(usersQueryInput.GetSkipIndex())
                .Take(usersQueryInput.GetPageSize())
                .ToList()
                .Select(x => _userViewFactory.Make(x.User, true, x.SightingCount))
                .ToPagedList(
                    usersQueryInput.GetPage(),
                    usersQueryInput.GetPageSize(),
                    stats.TotalResults
                );
        }

        #endregion

    }
}