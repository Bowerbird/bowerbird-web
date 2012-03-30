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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using System.Diagnostics;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Controllers.Members
{
    public class StreamItemController : ControllerBase
    {
        #region Members

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public StreamItemController(
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
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

            var groupContributions = _documentSession
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

            return groupContributions;
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

            var groupContributions = _documentSession
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

            return groupContributions;
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
                ObservationMedia = MakeObservationMediaItems(observation.Media)
            };
        }

        private IEnumerable<ObservationMediaItem> MakeObservationMediaItems(IEnumerable<ObservationMedia> observationMedia)
        {
            return observationMedia.Select(x =>
                new ObservationMediaItem()
                {
                    MediaResourceId = x.MediaResource.Id,
                    Description = x.Description,
                    Licence = x.Licence,
                    OriginalImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "original"),
                    LargeImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "large"),
                    MediumImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "medium"),
                    SmallImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "small"),
                    ThumbnailImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "thumbnail")
                });
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