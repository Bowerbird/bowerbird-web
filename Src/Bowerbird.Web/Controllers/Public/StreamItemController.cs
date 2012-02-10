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
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
	
namespace Bowerbird.Web.Controllers.Public
{
    public class StreamItemController : ControllerBase
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public StreamItemController(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            if (listInput.UserId != null)
            {
                return Json(MakeUserStreamItemList(listInput, sortInput));
            }

            if (listInput.GroupId != null)
            {
                return Json(MakeGroupStreamItemList(listInput, sortInput));
            }

            if (listInput.WatchlistId != null)
            {
                return Json(MakeWatchlistStreamItemList(listInput, sortInput));
            }

            return Json(null);
        }

        private StreamItemList MakeGroupStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId)
                .ToList();

            RavenQueryStatistics stats;
            var groupContributions = _documentSession
                .Query<GroupContributionResults, All_GroupContributionItems>()
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                .OrderByDescending(x => x.GroupCreatedDateTime)
                .AsProjection<GroupContributionResults>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            var streamItems = new StreamItemList()
            {
                StreamItems = groupContributions
                    .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        listInput.Page,
                        listInput.PageSize,
                        stats.TotalResults,
                        null)
            };

            return streamItems;
        }

        private StreamItemList MakeUserStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId)
                .ToList();

            RavenQueryStatistics stats;
            var groupContributions = _documentSession
                .Query<GroupContributionResults, All_GroupContributionItems>()
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                .OrderByDescending(x => x.GroupCreatedDateTime)
                .AsProjection<GroupContributionResults>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            var streamItems = new StreamItemList()
            {
                StreamItems = groupContributions
                    .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        listInput.Page,
                        listInput.PageSize,
                        stats.TotalResults,
                        null)
            };

            return streamItems;
        }

        private StreamItemList MakeWatchlistStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId)
                .ToList();

            RavenQueryStatistics stats;
            var groupContributions = _documentSession
                .Query<GroupContributionResults, All_GroupContributionItems>()
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)))
                .OrderByDescending(x => x.GroupCreatedDateTime)
                .AsProjection<GroupContributionResults>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            var streamItems = new StreamItemList()
            {
                StreamItems = groupContributions
                    .Select(x =>
                    new StreamItemViewModel()
                    {
                        Item = x,
                        ItemId = x.ContributionId,
                        SubmittedOn = x.GroupCreatedDateTime,
                        UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        listInput.Page,
                        listInput.PageSize,
                        stats.TotalResults,
                        null)
            };

            return streamItems;
        }

        #endregion
    }
}