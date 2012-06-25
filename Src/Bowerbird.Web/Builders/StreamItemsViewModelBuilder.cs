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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Config;
using System;
using Bowerbird.Core.Services;
using System.Dynamic;
using Raven.Abstractions.Linq;

namespace Bowerbird.Web.Builders
{
    public class StreamItemsViewModelBuilder : IStreamItemsViewModelBuilder
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public StreamItemsViewModelBuilder(
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

        public PagedList<object> BuildHomeStreamItems(StreamInput streamInput, PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groupIds = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .ToList()
                .SelectMany(x => x.GroupIds);

            var activityTypes = new [] 
            {
                "observationadded",
                "postadded",
                "observationnoteadded"
            };

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Statistics(out stats)
                .Where(x => x.GroupIds.Any(y => y.In(groupIds)) && x.Type.In(activityTypes));

            if (streamInput.NewerThan.HasValue)
            {
                query.Where(x => x.CreatedDateTime > streamInput.NewerThan.Value);
            }
            else if (streamInput.OlderThan.HasValue)
            {
                query.Where(x => x.CreatedDateTime < streamInput.OlderThan.Value);
            }

            return query
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.GetPageSize())
                .As<Activity>()
                .ToList()
                .Cast<object>()
                .ToPagedList(
                    pagingInput.GetPage(),
                    pagingInput.GetPageSize(),
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is User.Id
        /// </summary>
        public PagedList<object> BuildUserStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groupIds = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .ToList()
                .SelectMany(x => x.GroupIds);
            
            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groupIds))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.GetPageSize())
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(
                    pagingInput.GetPage(), 
                    pagingInput.GetPageSize(), 
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is Group.Id
        /// </summary>
        public PagedList<object> BuildGroupStreamItems(string groupId, StreamInput streamInput, PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Statistics(out stats)
                .Where(x => x.GroupIds.Any(y => y == groupId));

            if (streamInput.NewerThan.HasValue)
            {
                query.Where(x => x.CreatedDateTime > streamInput.NewerThan.Value);
            }
            else if (streamInput.OlderThan.HasValue)
            {
                query.Where(x => x.CreatedDateTime < streamInput.OlderThan.Value);
            }

            return query
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.GetPageSize())
                .As<Activity>()
                .ToList()
                .Cast<object>()
                .ToPagedList(
                    pagingInput.GetPage(),
                    pagingInput.GetPageSize(),
                    stats.TotalResults);
            //RavenQueryStatistics stats;

            //var groups = _documentSession
            //    .Query<All_Groups.Result, All_Groups>()
            //    .AsProjection<All_Groups.Result>()
            //    .Where(x => x.GroupId == pagingInput.Id)
            //    .ToList()
            //    .SelectMany(x => x.AncestorGroupIds)
            //    .Union(new [] { pagingInput.Id });

            //return _documentSession
            //    .Query<All_Contributions.Result, All_Contributions>()
            //    .AsProjection<All_Contributions.Result>()
            //    .Statistics(out stats)
            //    .Include(x => x.ContributionId)
            //    .Where(x => x.GroupId.In(groups))
            //    .OrderByDescending(x => x.CreatedDateTime)
            //    .Skip(pagingInput.GetSkipIndex())
            //    .Take(pagingInput.GetPageSize())
            //    .ToList()
            //    .Select(MakeStreamItem)
            //    .ToPagedList(
            //        pagingInput.GetPage(),
            //        pagingInput.GetPageSize(), 
            //        stats.TotalResults);
        }

        //private dynamic MakeStreamItem2(All_Activities.Result result)
        //{
        //    dynamic streamItem = new ExpandoObject();

        //    streamItem.Type = result.ActivityType;
        //    streamItem.CreatedDateTime = result.CreatedDateTime;
        //    streamItem.CreatedDateTimeDescription = MakeCreatedDateTimeDescription(result.CreatedDateTime);
        //    streamItem.Groups = result.GroupIds;
        //    streamItem.User = new
        //    {
        //        result.User.Id,
        //        result.User.LastLoggedIn,
        //        Name = result.User.GetName(),
        //        Avatar = _avatarFactory.Make(result.User)
        //    };

        //    switch (result.ActivityType)
        //    {
        //        case "observationadded":
        //            var observation = result.Activity.Content as Observation;
        //            streamItem.Description = string.Format("{0} {1}", result.User.FirstName, "added an observation");
        //            streamItem.ObservationAdded = new 
        //            {
        //                Id = observation.ShortId(),
        //                Title = observation.Title,
        //                ObservedOn = observation.ObservedOn.ToString("d MMM yyyy"),
        //                Address = observation.Address,
        //                Latitude = observation.Latitude,
        //                Longitude = observation.Longitude,
        //                Category = observation.Category,
        //                IsIdentificationRequired = observation.IsIdentificationRequired,
        //                AnonymiseLocation = observation.AnonymiseLocation,
        //                Media = observation.Media.Select(MakeObservationMediaItem),
        //                Projects = observation.Groups.Select(x => x.Group.Id)
        //            };
        //            break;
        //        case "userjoinedgroup":
        //            var member = result.Activity.Content as Member;
        //            var group = _documentSession.Load<dynamic>(member.Group.Id);
        //            streamItem.Description = string.Format("{0} joined {1}", result.User.FirstName, group.Name);
        //            streamItem.UserJoinedGroup = new
        //            {
        //                Id = member.ShortId(),
        //                User = result.User,
        //                Group = new
        //                {
        //                    group.Id,
        //                    group.GroupType,
        //                    group.Name,
        //                    Avatar = _avatarFactory.Make(group)
        //                }
        //            };
        //            break;
        //        default:
        //            throw new NotImplementedException();
        //    }

        //    return streamItem;
        //}

        //private object MakeObservationMediaItem(ObservationMedia observationMedia)
        //{
        //    return new
        //    {
        //        MediaResourceId = observationMedia.MediaResource.Id,
        //        observationMedia.Description,
        //        observationMedia.Licence,
        //        observationMedia.MediaResource.Metadata,
        //        observationMedia.MediaResource.Type,
        //        CreatedByUser = observationMedia.MediaResource.CreatedByUser.Id,
        //        UploadedOn = observationMedia.MediaResource.UploadedOn.ToString("d MMM yyyy"),
        //        OriginalImageUri = _mediaFilePathService.MakeMediaFileUri(observationMedia.MediaResource, "original"),
        //        LargeImageUri = _mediaFilePathService.MakeMediaFileUri(observationMedia.MediaResource, "large"),
        //        MediumImageUri = _mediaFilePathService.MakeMediaFileUri(observationMedia.MediaResource, "medium"),
        //        SmallImageUri = _mediaFilePathService.MakeMediaFileUri(observationMedia.MediaResource, "small"),
        //        ThumbnailImageUri = _mediaFilePathService.MakeMediaFileUri(observationMedia.MediaResource, "thumbnail")
        //    };
        //}

        private object MakeStreamItem(All_Contributions.Result groupContributionResult)
        {
            object item = null;
            string description = null;
            IEnumerable<string> groups = null;

            switch (groupContributionResult.ContributionType)
            {
                case "Observation":
                    item = new
                    {
                        ItemType = "observation",
                        Id = groupContributionResult.Observation.ShortId(),
                        Title = groupContributionResult.Observation.Title,
                        ObservedOn = groupContributionResult.Observation.ObservedOn.ToString("d MMM yyyy"),
                        Address = groupContributionResult.Observation.Address,
                        Latitude = groupContributionResult.Observation.Latitude,
                        Longitude = groupContributionResult.Observation.Longitude,
                        Category = groupContributionResult.Observation.Category,
                        IsIdentificationRequired = groupContributionResult.Observation.IsIdentificationRequired,
                        AnonymiseLocation = groupContributionResult.Observation.AnonymiseLocation,
                        Media = groupContributionResult.Observation.Media.Select(x => MakeObservationMediaItem(x, groupContributionResult.Observation.GetPrimaryImage() == x)),
                        PrimaryImage = MakeObservationMediaItem(groupContributionResult.Observation.GetPrimaryImage(), true),
                        Projects = groupContributionResult.Observation.Groups.Select(x => x.Group.Id)
                    };
                    description = groupContributionResult.Observation.User.FirstName + " added an observation";
                    groups = groupContributionResult.Observation.Groups.Select(x => x.Group.Id);
                    break;
            }

            return new
            {
                CreatedDateTime = groupContributionResult.GroupCreatedDateTime,
                CreatedDateTimeDescription = MakeCreatedDateTimeDescription(groupContributionResult.GroupCreatedDateTime),
                Type = groupContributionResult.ContributionType.ToLower(),
                User = new
                {
                    groupContributionResult.GroupUser.Id,
                    groupContributionResult.GroupUser.LastLoggedIn,
                    Name = groupContributionResult.GroupUser.GetName(),
                    Avatar = groupContributionResult.GroupUser.Avatar
                },
                Item = item,
                Description = description,
                Groups = groups
            };
        }

        private object MakeObservationMediaItem(ObservationMedia observationMedia, bool isPrimaryImage)
        {
            if (observationMedia.MediaResource.Type == "image")
            {
                return new
                {
                    IsPrimaryImage = isPrimaryImage,
                    MediaResourceId = observationMedia.MediaResource.Id,
                    observationMedia.MediaResource.Type,
                    observationMedia.Description,
                    observationMedia.Licence,
                    CreatedByUser = observationMedia.MediaResource.CreatedByUser.Id,
                    UploadedOn = observationMedia.MediaResource.UploadedOn,
                    OriginalImage = observationMedia.MediaResource.Files["original"],
                    LargeImage = observationMedia.MediaResource.Files["large"],
                    MediumImage = observationMedia.MediaResource.Files["medium"],
                    SmallImage = observationMedia.MediaResource.Files["small"],
                    ThumbnailImage = observationMedia.MediaResource.Files["thumbnail"]
                };
            }

            throw new NotImplementedException();
        }

        private static string MakeCreatedDateTimeDescription(DateTime dateTime)
        {
            var diff = DateTime.UtcNow.Subtract(dateTime);

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

        #endregion
    }
}