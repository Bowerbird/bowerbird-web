/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Web.Factories;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class StreamItemController : ControllerBase
    {
        #region Members

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IStreamItemFactory _streamItemFactory;
        private readonly IObservationViewFactory _observationViewFactory;

        #endregion

        #region Constructors

        public StreamItemController(
            IUserContext userContext,
            IDocumentSession documentSession,
            IStreamItemFactory streamItemFactory,
            IObservationViewFactory observationViewFactory)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");
            Check.RequireNotNull(observationViewFactory, "observationViewFactory");

            _userContext = userContext;
            _documentSession = documentSession;
            _streamItemFactory = streamItemFactory;
            _observationViewFactory = observationViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            //if (listInput.UserId != null)
            //{
            //    return Json(MakeUserStreamItemList(listInput, sortInput), JsonRequestBehavior.AllowGet);
            //}

            if (listInput.GroupId != null)
            {
                return new JsonNetResult(MakeGroupStreamItemList(listInput, sortInput));
            }

            //if (listInput.WatchlistId != null)
            //{
            //    return Json(MakeWatchlistStreamItemList(listInput, sortInput));
            //}

            return new JsonNetResult(MakeHomeStreamItemList(listInput, sortInput));
        }

        private PagedList<StreamItem> MakeHomeStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<Member>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x => new { GroupId = x.Group.Id })
                .ToList();

            return _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((listInput.Page - 1) * listInput.PageSize)
                .Take(listInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(listInput.Page, listInput.PageSize, stats.TotalResults);
        }

        private PagedList<StreamItem> MakeGroupStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<GroupAssociation>()
                .Where(x => x.ParentGroupId == listInput.GroupId)
                .ToList()
                .Select(x => new { GroupId = x.ChildGroupId })
                .ToList();

            groups.Add(new { GroupId = listInput.GroupId });

            return _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((listInput.Page - 1) * listInput.PageSize)
                .Take(listInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(listInput.Page, listInput.PageSize, stats.TotalResults);
        }

        private StreamItem MakeStreamItem(All_GroupContributions.Result groupContributionResult)
        {
            object item = null;
            string description = null;
            IEnumerable<string> groups = null;

            switch (groupContributionResult.ContributionType)
            {
                case "Observation":
                    item = _observationViewFactory.Make(groupContributionResult.Observation);
                    description = groupContributionResult.Observation.User.FirstName + " added an observation";
                    groups = groupContributionResult.Observation.Groups.Select(x => x.GroupId);
                    break;
            }

            return _streamItemFactory.Make(
                item,
                groups,
                "observation",
                groupContributionResult.GroupUser,
                groupContributionResult.GroupCreatedDateTime,
                description);
        }

        //// Get all stream items for all groups that a particular user is a member of
        //private PagedList<StreamItem> MakeUserStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        //{
        //    RavenQueryStatistics stats;

        //    var groupMemberships = _documentSession
        //        .Query<Member>()
        //        .Include(x => x.User.Id)
        //        .Where(x => x.User.Id == listInput.UserId);
        //        //.ToList();

        //    var groupContributions = GetContributionsForGroups(groupMemberships)
        //        .AsProjection<All_GroupContributions.Result>()
        //        .Include(x => x.ContributionId)
        //        .Where(x => x.UserId == listInput.UserId)
        //        .Statistics(out stats)
        //        .Skip(listInput.Page)
        //        .Take(listInput.PageSize)
        //        .ToList();

        //    //SortResults(groupContributions, sortInput);

        //    //RavenQueryStatistics stats = ProjectGroupContributions(listInput, groupContributions);

        //    return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);
        //}

        //// Get all items that match the query of a users' watchlist
        //private StreamItemList MakeWatchlistStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        //{
        //    var memberWatchlists = _documentSession.Load<Watchlist>(listInput.WatchlistId);

        //    // TODO: Create an index to query for watchlist 

        //    //var groupContributions = _documentSession
        //    //    .Query<GroupContributionResults, All_GroupContributionItems>()
        //    //    .Include(x => x.ContributionId)
        //    //    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)));

        //    //SortResults(groupContributions, sortInput);

        //    //RavenQueryStatistics stats;

        //    //groupContributions
        //    //    .OrderByDescending(x => x.GroupCreatedDateTime)
        //    //    .AsProjection<GroupContributionResults>()
        //    //    .Statistics(out stats)
        //    //    .Skip(listInput.Page)
        //    //    .Take(listInput.PageSize)
        //    //    .ToList();

        //    //return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);

        //    return new StreamItemList();
        //}

        #endregion
    }
}