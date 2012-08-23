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
using Bowerbird.Web.Factories;

namespace Bowerbird.Web.Builders
{
    public class TeamViewModelBuilder : ITeamViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public TeamViewModelBuilder(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildNewTeam()
        {
            return new
            {
                Name = string.Empty,
                Description = string.Empty,
                Website = string.Empty,
                Avatar = _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Team),
                MemberCount = 1
            };
        }
        
        public object BuildTeam(string teamId)
        {
            Check.RequireNotNullOrWhitespace(teamId, "teamId");

            var team = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .FirstOrDefault(x => x.GroupId == teamId);

            return _groupViewFactory.Make(team);
        }

        public object BuildUserTeamList(string userId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.UserIds.Any(y => y == userId))
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_groupViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        public object BuildGroupTeamList(string groupId, bool getAllDescendants, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .AsProjection<All_Groups.Result>();

            if (groupId == Constants.AppRootId)
            {
                if (getAllDescendants)
                {
                    query = query
                        .Where(x => x.GroupType == "team");
                }
                else
                {
                    query = query
                        .Where(x => x.GroupType == "team" && x.ParentGroupId == Constants.AppRootId);
                }
            }
            else
            {
                var group = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .AsProjection<All_Groups.Result>()
                    .Where(x => x.GroupId == groupId)
                    .ToList()
                    .First();

                var descendantGroupIds = getAllDescendants ? group.DescendantGroupIds : group.ChildGroupIds;

                query = query
                    .Where(x => x.GroupId.In(descendantGroupIds) && x.GroupType == "team");
            }

            RavenQueryStatistics stats;

            return query
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_groupViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}