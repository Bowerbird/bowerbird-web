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
using System.Linq.Expressions;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using System.Diagnostics;

namespace Bowerbird.Web.Controllers.Members
{
    public class StreamItemController : ControllerBase
    {
        #region Members

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public StreamItemController(
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            if (listInput.UserId != null)
            {
                return Json(MakeUserStreamItemList(listInput, sortInput), JsonRequestBehavior.AllowGet);
            }

            if (listInput.GroupId != null)
            {
                return Json(MakeGroupStreamItemList(listInput, sortInput), JsonRequestBehavior.AllowGet);
            }

            if (listInput.WatchlistId != null)
            {
                return Json(MakeWatchlistStreamItemList(listInput, sortInput));
            }

            return Json(MakeHomeStreamItemList(listInput, sortInput), JsonRequestBehavior.AllowGet);
        }

        private PagedList<StreamItem> MakeHomeStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            RavenQueryStatistics stats;

            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x => new { GroupId = x.Group.Id })
                .ToList();

            var groupContributions = _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupMemberships.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Statistics(out stats)
                .Skip((listInput.Page - 1) * listInput.PageSize)
                .Take(listInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(listInput.Page, listInput.PageSize, stats.TotalResults);

            return groupContributions;

            //SortResults(groupContributions, sortInput);

            //return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);
        }

        private PagedList<StreamItem> MakeGroupStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            RavenQueryStatistics stats;

            // get all group and child groups
            var group = _documentSession.Load<Group>(listInput.GroupId);
            var groupIds = group.ChildGroupAssociations.Select(x => new { x.GroupId }).ToList();
            groupIds.Add(new { GroupId = group.Id });

            var groupContributions = _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupIds.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(listInput.Page, listInput.PageSize, stats.TotalResults);

            return groupContributions;

            //SortResults(groupContributions, sortInput);

            //return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);
        }

        private StreamItem MakeStreamItem(All_GroupContributions.Result groupContributionResult)
        {
            var streamItem = new StreamItem()
                    {
                        CreatedDateTime = groupContributionResult.CreatedDateTime,
                        CreatedDateTimeDescription = MakeCreatedDateTimeDescription(groupContributionResult.CreatedDateTime),
                        Type = groupContributionResult.ContributionType,
                        User = groupContributionResult.GroupUserId
                    };

            switch (groupContributionResult.ContributionType)
            {
                case "Observation":
                    streamItem.Item = MakeObservationView(groupContributionResult.Observation);
                    streamItem.Description = groupContributionResult.Observation.User.FirstName + " added an observation";
                    break;
            }

            return streamItem;
        }

        private string MakeCreatedDateTimeDescription(DateTime dateTime)
        {
            var diff = DateTime.Now.Subtract(dateTime);

            if (diff > new TimeSpan(365, 0, 0, 0)) // Year
            {
                return "more than a year ago";
            } 
            else if (diff > new TimeSpan(30, 0, 0, 0)) // Month
            {
                var months = (diff.Days / 30);
                return string.Format("{0} month{1} ago", months, months > 1 ? "s" : string.Empty);
            } 
            else if (diff > new TimeSpan(1, 0, 0, 0)) // Day
            {
                return string.Format("{0} day{1} ago", diff.Days, diff.Days > 1 ? "s" : string.Empty);
            }
            else if (diff > new TimeSpan(1, 0, 0)) // Hour
            {
                return string.Format("{0} hour{1} ago", diff.Hours, diff.Hours > 1 ? "s" : string.Empty);
            }
            else if (diff > new TimeSpan(0, 1, 0)) // Minute
            {
                return string.Format("{0} minute{1} ago", diff.Minutes, diff.Minutes > 1 ? "s" : string.Empty);
            }
            else // Second
            {
                return "just now";
            }
        }

        private ObservationView MakeObservationView(Observation observation)
        {
            return new ObservationView()
            {
                Id = observation.Id,
                Title = observation.Title,
                ObservedOn = observation.ObservedOn,
                Address = observation.Address,
                Latitude = observation.Latitude,
                Longitude = observation.Longitude,
                ObservationCategory = observation.ObservationCategory,
                IsIdentificationRequired = observation.IsIdentificationRequired,
                ObservationMedia = observation.Media
            };
        }

        // Get all stream items for all groups that a particular user is a member of
        private PagedList<StreamItem> MakeUserStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            RavenQueryStatistics stats;

            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == listInput.UserId);
                //.ToList();

            var groupContributions = GetContributionsForGroups(groupMemberships)
                .AsProjection<All_GroupContributions.Result>()
                .Include(x => x.ContributionId)
                .Where(x => x.UserId == listInput.UserId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            //SortResults(groupContributions, sortInput);

            //RavenQueryStatistics stats = ProjectGroupContributions(listInput, groupContributions);

            return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);
        }

        // Get all items that match the query of a users' watchlist
        private StreamItemList MakeWatchlistStreamItemList(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            var memberWatchlists = _documentSession.Load<Watchlist>(listInput.WatchlistId);

            // TODO: Create an index to query for watchlist 

            //var groupContributions = _documentSession
            //    .Query<GroupContributionResults, All_GroupContributionItems>()
            //    .Include(x => x.ContributionId)
            //    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)));

            //SortResults(groupContributions, sortInput);

            //RavenQueryStatistics stats;

            //groupContributions
            //    .OrderByDescending(x => x.GroupCreatedDateTime)
            //    .AsProjection<GroupContributionResults>()
            //    .Statistics(out stats)
            //    .Skip(listInput.Page)
            //    .Take(listInput.PageSize)
            //    .ToList();

            //return SetStreamItemList(groupContributions, stats, listInput.Page, listInput.PageSize);

            return new StreamItemList();
        }

        private PagedList<StreamItem> SetStreamItemList(IEnumerable<All_GroupContributions.Result> results, RavenQueryStatistics stats, int page, int pageSize)
        {
            return results
                    .Select(x =>
                    new StreamItem()
                    {
                        CreatedDateTime = x.CreatedDateTime,
                        Type = "thing",
                        User = x.UserId,
                        Description = "description",
                        Item = x,
                        
                        //ItemId = x.ContributionId,
                        //SubmittedOn = x.GroupCreatedDateTime,
                        //UserId = x.GroupUserId
                    }
                    ).ToPagedList(
                        page,
                        pageSize,
                        stats.TotalResults,
                        null);
        }

        private IRavenQueryable<All_GroupContributions.Result> GetContributionsForGroups(IEnumerable<GroupMember> groupMemberships)
        {
            var groupContributions = _documentSession
                    .Query<All_GroupContributions.Result, All_GroupContributions>()
                    .Include(x => x.ContributionId)
                    .Where(x => x.GroupId.In(groupMemberships.Select(y => y.Group.Id)));

            return groupContributions;
        }

        private void SortResults(IQueryable<All_GroupContributions.Result> query, StreamSortInput sortInput)
        {
            if (sortInput.DateTimeDescending)
            {
                query.OrderByDescending(x => x.GroupCreatedDateTime);
            }
            else
            {
                query.OrderBy(x => x.GroupCreatedDateTime);
            }
        }

        private static RavenQueryStatistics ProjectGroupContributions(StreamItemListInput listInput, IRavenQueryable<All_GroupContributions.Result> groupContributions)
        {
            RavenQueryStatistics stats;

            groupContributions
                .AsProjection<All_GroupContributions.Result>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            return stats;
        }

        #endregion
    }
}