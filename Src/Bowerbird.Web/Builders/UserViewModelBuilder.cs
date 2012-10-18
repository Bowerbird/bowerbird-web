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
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using NodaTime;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.Builders
{
    public class UserViewModelBuilder : IUserViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public UserViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
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

        public object BuildEditableUser(string userId)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");

            var user = _documentSession.Load<User>(userId);

            return new
            {
                user.Id,
                user.Avatar,
                user.LastLoggedIn,
                user.Name,
                user.Email
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
                Organisations = groupResults.Where(x => x.Group is Organisation).Select(x => _groupViewFactory.Make(x)),
                Teams = groupResults.Where(x => x.Group is Team).Select(x => _groupViewFactory.Make(x)),
                Projects = groupResults.Where(x => x.Group is Project).Select(x => _groupViewFactory.Make(x)),
                UserProjects = groupResults.Where(x => x.Group is UserProject).Select(x => _groupViewFactory.Make(x)),
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

        public object BuildGroupUserList(string groupId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            return _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.GroupIds.Any(y => y == groupId))
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _userViewFactory.Make(x.User));
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

        private object MakeMember(Member member)
        {
            return new
            {
                GroupId = member.Group.Id,
                Roles = from role in member.Roles
                        select new
                        {
                            Id = role.Id,
                            Permissions = role.Permissions.Select(x => x.Id)
                        }
            };
        }

        #endregion

    }
}