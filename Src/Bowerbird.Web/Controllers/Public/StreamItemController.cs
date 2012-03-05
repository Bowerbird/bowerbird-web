///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/

//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using Bowerbird.Core.DesignByContract;
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.DomainModels.Members;
//using Bowerbird.Core.Indexes;
//using Bowerbird.Core.Paging;
//using Bowerbird.Web.ViewModels.Shared;
//using Raven.Client;
//using Raven.Client.Linq;
	
//namespace Bowerbird.Web.Controllers.Public
//{
//    public class StreamItemController : ControllerBase
//    {
//        #region Members

//        private readonly IDocumentSession _documentSession;

//        #endregion

//        #region Constructors

//        public StreamItemController(
//            IDocumentSession documentSession)
//        {
//            Check.RequireNotNull(documentSession, "documentSession");

//            _documentSession = documentSession;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        [HttpGet]
//        public ActionResult List(StreamItemListInput listInput, StreamSortInput sortInput)
//        {
//            if (listInput.UserId != null)
//            {
//                return Json(MakeUserStreamItemList(listInput, sortInput));
//            }

//            if (listInput.GroupId != null)
//            {
//                return Json(MakeGroupStreamItemList(listInput, sortInput));
//            }

//            if (listInput.WatchlistId != null)
//            {
//                return Json(MakeWatchlistStreamItemList(listInput, sortInput));
//            }

//            return Json(null);
//        }

//        // get all stream items for a particular group
//        private StreamItemList MakeGroupStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
//        {
//            RavenQueryStatistics stats;

//            var groupContributions = _documentSession
//                .Query<All_GroupContributions.GroupContributionReduceResult, All_GroupContributions>()
//                .Include(x => x.ContributionId)
//                .Where(x => x.GroupId == listInput.GroupId)
//                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList();

//            //SortResults(groupContributions, sortInput);

//            //RavenQueryStatistics stats = ProjectGroupContributions(listInput, groupContributions);

//            return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);
//        }

//        // Get all stream items for all groups that a particular user is a member of
//        private StreamItemList MakeUserStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
//        {
//            var groupMemberships = _documentSession
//                .Query<GroupMember>()
//                .Include(x => x.User.Id)
//                .Where(x => x.User.Id == listInput.UserId)
//                .ToList();

//            var groupContributions = GetContributionsForGroups(groupMemberships);

//            SortResults(groupContributions, sortInput);

//            RavenQueryStatistics stats = ProjectGroupContributions(listInput, groupContributions);

//            return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);
//        }

//        // Get all items that match the query of a users' watchlist
//        private StreamItemList MakeWatchlistStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
//        {
//            var memberWatchlists = _documentSession.Load<Watchlist>(listInput.WatchlistId);

//            // TODO: Create an index to query for watchlist 

//            //var groupContributions = _documentSession
//            //    .Query<GroupContributionResults, All_GroupContributionItems>()
//            //    .Include(x => x.ContributionId)
//            //    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)));

//            //SortResults(groupContributions, sortInput);

//            //RavenQueryStatistics stats;

//            //groupContributions
//            //    .OrderByDescending(x => x.GroupCreatedDateTime)
//            //    .AsProjection<GroupContributionResults>()
//            //    .Statistics(out stats)
//            //    .Skip(listInput.Page)
//            //    .Take(listInput.PageSize)
//            //    .ToList();

//            //return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);

//            return new StreamItemList();
//        }

//        private StreamItemList SetStreamItemList(IEnumerable<All_GroupContributions.GroupContributionReduceResult> results, RavenQueryStatistics stats, int page, int pageSize)
//        {
//            return new StreamItemList()
//            {
//                StreamItems = results
//                    .Select(x =>
//                    new StreamItem()
//                    {
//                        Item = x,
//                        //ItemId = x.ContributionId,
//                        //SubmittedOn = x.GroupCreatedDateTime,
//                        //UserId = x.GroupUserId
//                    }
//                    ).ToPagedList(
//                        page,
//                        pageSize,
//                        stats.TotalResults,
//                        null)
//            };
//        }

//        private IRavenQueryable<All_GroupContributions.GroupContributionReduceResult> GetContributionsForGroups(IEnumerable<GroupMember> groupMemberships)
//        {
//            var groupContributions = _documentSession
//                    .Query<All_GroupContributions.GroupContributionReduceResult, All_GroupContributions>()
//                    .Include(x => x.ContributionId)
//                    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)));

//            return groupContributions;
//        }

//        private void SortResults(IQueryable<All_GroupContributions.GroupContributionReduceResult> query, StreamSortInput sortInput)
//        {
//            if (sortInput.DateTimeDescending)
//            {
//                query.OrderByDescending(x => x.GroupCreatedDateTime);
//            }
//            else
//            {
//                query.OrderBy(x => x.GroupCreatedDateTime);
//            }
//        }

//        private static RavenQueryStatistics ProjectGroupContributions(StreamItemListInput listInput, IRavenQueryable<All_GroupContributions.GroupContributionReduceResult> groupContributions)
//        {
//            RavenQueryStatistics stats;

//            groupContributions
//                .AsProjection<All_GroupContributions.GroupContributionReduceResult>()
//                .Statistics(out stats)
//                .Skip(listInput.Page)
//                .Take(listInput.PageSize)
//                .ToList();

//            return stats;
//        }


//        #endregion
//    }
//}