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
using Bowerbird.Core.Services;
using Bowerbird.Core.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.ViewModelFactories;

namespace Bowerbird.Core.Queries
{
    public class UserViewModelQuery : IUserViewModelQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IUserContext _userContext;
        private readonly IDateTimeZoneService _dateTimeZoneService;

        #endregion

        #region Constructors

        public UserViewModelQuery(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IUserContext userContext,
            IDateTimeZoneService dateTimeZoneService)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(dateTimeZoneService, "dateTimeZoneService");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _userContext = userContext;
            _dateTimeZoneService = dateTimeZoneService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildUser(string userId, bool fullDetails = false)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");

            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            return _userViewFactory.Make(_documentSession.Load<User>(userId), authenticatedUser, fullDetails);
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
                User = _userViewFactory.Make(user, user),
                AppRoot = groupResults.Where(x => x.Group is AppRoot).Select(x => x.Group as AppRoot).First(),
                Organisations = groupResults.Where(x => x.Group is Organisation).Select(x => _groupViewFactory.Make(x.Group, user)),
                Teams = groupResults.Where(x => x.Group is Team).Select(x => _groupViewFactory.Make(x.Group, user)),
                Projects = groupResults.Where(x => x.Group is Project).Select(x => _groupViewFactory.Make(x.Group, user)),
                // Get all userprojects that are followed, except the user's own userproject
                UserProjects = groupResults.Where(x => x.Group is UserProject && x.Group.User.Id != userId).Select(x => _groupViewFactory.Make(x.Group, user)),
                Memberships = user.Memberships.Select(x => new {
                    GroupId = x.Group.Id,
                    x.Group.GroupType,
                    RoleIds = x.Roles.Select(y => y.Id),
                    PermissionIds = x.Roles.SelectMany(y => y.Permissions).Select(y => y.Id)
                }),
                user.DefaultLicence,
                TimezoneOffset = _dateTimeZoneService.GetOffsetFromUtcNow(user.Timezone),
                user.CallsToAction
            };
        }

        public object BuildOnlineUserList(string authenticatedUserId)
        {
            // Return connected users (those users active less than 5 minutes ago)
            var fiveMinutesAgo = DateTime.UtcNow - TimeSpan.FromMinutes(5);

            User authenticatedUser = _documentSession.Load<User>(authenticatedUserId);

            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.LatestHeartbeat.Any(y => y > fiveMinutesAgo))
                .Take(100) //HACK: Need to work out how we will list more than RavenDB max
                .ToList()
                .Select(x => _userViewFactory.Make(x.User, authenticatedUser));
        }

        public object BuildGroupUserList(string groupId, string role)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");

            RavenQueryStatistics stats;

            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            return _documentSession
                .Advanced.LuceneQuery<All_Users.Result, All_Users>()
                .SelectFields<All_Users.Result>("UserId", "GroupIds", "ConnectionIds", "SightingCount", "LatestHeartbeat", "LatestActivity")
                .WhereEquals(role.Replace("roles/", ""), groupId)
                .OrderBy(x => x.Name)
                .Statistics(out stats)
                .Take(100)
                .ToList()
                .Select(x => _userViewFactory.Make(x.User, authenticatedUser, true, x.SightingCount))
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

            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Users.Result, All_Users>()
                .Statistics(out stats)
                .SelectFields<All_Users.Result>("UserId", "GroupIds", "ConnectionIds", "SightingCount", "LatestHeartbeat", "LatestActivity", "User", "LatestObservationIds", "LatestObservations");

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
                case "z-a":
                    query = query.AddOrder(x => x.SortName, true);
                    break;
                default:
                case "a-z":
                    query = query.AddOrder(x => x.SortName, false);
                    break;
            }

            return query
                .Skip(usersQueryInput.GetSkipIndex())
                .Take(usersQueryInput.GetPageSize())
                .ToList()
                .Select(x => _userViewFactory.Make(x.User, authenticatedUser, true, x.SightingCount, x.LatestObservations != null ? x.LatestObservations.Take(4) : null))
                .ToPagedList(
                    usersQueryInput.GetPage(),
                    usersQueryInput.GetPageSize(),
                    stats.TotalResults
                );
        }

        #endregion

    }
}