/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.Factories;

namespace Bowerbird.Web.Builders
{
    public class TeamsViewModelBuilder : ITeamsViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public TeamsViewModelBuilder(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildTeam(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            var team = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .FirstOrDefault(x => x.GroupId == idInput.Id);

            return MakeTeam(team);
        }

        public object BuildTeam()
        {
            return new
            {
                Name = "New Team",
                Description = "New Team",
                Website = "",
                Avatar = _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Team),
                MemberCount = 1
            };
        }

        public object BuildTeamList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.GroupId)
                .Where(x => x.GroupType == "team")
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeTeam)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is User.Id
        /// </summary>
        public object BuildUserTeamList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var teams = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == pagingInput.Id)
                .ToList()
                .SelectMany(x => x.Members.Where(y => y.Group.GroupType == "team").Select(y => y.Group.Id));
                
            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId.In(teams))
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeTeam)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is Organisation.Id
        /// </summary>
        public object BuildOrganisationTeamList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var organisationTeams = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroup.Id == pagingInput.Id && x.ChildGroup.GroupType == "team")
                .Include(x => x.ChildGroup.Id);

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId.In(organisationTeams.Select(y => y.ChildGroup.Id)))
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeTeam)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is Team.Id
        /// </summary>
        public object BuildTeamUserList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<Member>()
                .Where(x => x.Group.Id == pagingInput.Id)
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.User.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => MakeUser(x.User.Id))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        private object MakeUser(string userId)
        {
            return MakeUser(_documentSession.Load<User>(userId));
        }

        private object MakeUser(User user)
        {
            return new
            {
                Avatar = user.Avatar,
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        private object MakeTeam(All_Groups.Result result)
        {
            return new
            {
                Id = result.Team.Id,
                result.Team.Name,
                result.Team.Description,
                result.Team.Website,
                Avatar = result.Team.Avatar,
                MemberCount = result.MemberIds.Count()
                //Projects = result.ChildGroups.Where(x => x.GroupType == "project").Select(x => x.Id)
            };
        }

        #endregion
    }
}