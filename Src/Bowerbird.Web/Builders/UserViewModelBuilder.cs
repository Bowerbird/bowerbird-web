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
        private readonly IUserViewFactory _userViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public UserViewModelBuilder(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IUserContext userContext
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildItem(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _userViewFactory.Make(idInput.Id);
        }

        public object BuildList(PagingInput pagingInput)
        {
            Check.RequireNotNull(listInput, "listInput");

            return BuildUsers(listInput);
        }

        private object BuildUsers(PagingInput pagingInput)
        {
            var userMemberships = _documentSession.Query<All_UserMemberships.Result, All_UserMemberships>()
                .Include(x => x.GroupId)
                .Where(x => x.GroupId == listInput.GroupId)
                .Distinct()
                .ToList();

            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<User>()
                .Where(x => x.Id.In(userMemberships.Select(y => y.UserId)))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _userViewFactory.Make(x.Id));

            return new
            {
                listInput.Page,
                listInput.PageSize,
                Users = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildAuthenticatedUser()
        {
            return BuildItem(new IdInput() { Id = _userContext.GetAuthenticatedUserId() });
        }

        public IEnumerable BuildOnlineUsers()
        {
            var connectedUserIds = _documentSession
                .Query<All_UserSessions.Results, All_UserSessions>()
                .AsProjection<All_UserSessions.Results>()
                .Include(x => x.UserId)
                .Where(x => x.Status < (int)Connection.ConnectionStatus.Offline)
                .ToList()
                .Select(x => x.UserId)
                .Distinct();

            var connectedUsers = _documentSession.Query<User>()
                .Where(x => x.Id.In(connectedUserIds))
                .ToList();

            return connectedUsers
                .Select(x => _userViewFactory.Make(x.Id))
                .ToList();
        }

        //private UserProfile MakeUserProfile(User user, IEnumerable<Member> memberships)
        //{
        //    return new UserProfile()
        //    {
        //        Id = user.Id,
        //        Name = user.GetName(),
        //        LastLoggedIn = user.LastLoggedIn,
        //        Avatar = _avatarFactory.Make(user),
        //        Projects = _documentSession.Load<Project>(memberships.Where(x => x.Group.Id.StartsWith("projects/")).Select(x => x.Group.Id)).Select(x => _projectViewFactory.Make(x)),
        //        Teams = _documentSession.Load<Team>(memberships.Where(x => x.Group.Id.StartsWith("teams/")).Select(x => x.Group.Id)).Select(x => _teamViewFactory.Make(x))
        //    };
        //}

        #endregion
    }
}