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
using Bowerbird.Web.Factories;
using System;

namespace Bowerbird.Web.Builders
{
    public class StreamItemsViewModelBuilder : IStreamItemsViewModelBuilder
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public StreamItemsViewModelBuilder(
            IUserContext userContext,
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _userContext = userContext;
            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public PagedList<object> BuildHomeStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<Member>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .ToList()
                .Select(x => new { GroupId = x.Group.Id });

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups.Select(y => y.GroupId)))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(
                    pagingInput.Page, 
                    pagingInput.PageSize, 
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is User.Id
        /// </summary>
        public PagedList<object> BuildUserStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.UserId == _userContext.GetAuthenticatedUserId())
                .ToList()
                .SelectMany(x => x.Members.Select(y => y.Group.Id));

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(
                    pagingInput.Page, 
                    pagingInput.PageSize, 
                    stats.TotalResults);
        }

        /// <summary>
        /// PagingInput.Id is Group.Id
        /// </summary>
        public PagedList<object> BuildGroupStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var groups = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == pagingInput.Id)
                .ToList()
                .SelectMany(x => x.AncestorGroupIds)
                .Union(new [] { pagingInput.Id });

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.GroupId.In(groups))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip((pagingInput.Page - 1) * pagingInput.PageSize)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeStreamItem)
                .ToPagedList(
                    pagingInput.Page, 
                    pagingInput.PageSize, 
                    stats.TotalResults);
        }

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
                        Id = groupContributionResult.Observation.ShortId(),
                        Title = groupContributionResult.Observation.Title,
                        ObservedOn = groupContributionResult.Observation.ObservedOn.ToString("d MMM yyyy"),
                        Address = groupContributionResult.Observation.Address,
                        Latitude = groupContributionResult.Observation.Latitude,
                        Longitude = groupContributionResult.Observation.Longitude,
                        Category = groupContributionResult.Observation.Category,
                        IsIdentificationRequired = groupContributionResult.Observation.IsIdentificationRequired,
                        AnonymiseLocation = groupContributionResult.Observation.AnonymiseLocation,
                        //Media = MakeObservationMediaItems(groupContributionResult.Observation.Media),
                        Projects = groupContributionResult.Observation.Groups.Select(x => x.GroupId)
                    };
                    description = groupContributionResult.Observation.User.FirstName + " added an observation";
                    groups = groupContributionResult.Observation.Groups.Select(x => x.GroupId);
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
                    Avatar = _avatarFactory.Make(groupContributionResult.GroupUser)
                },
                Item = item,
                Description = description,
                Groups = groups
            };

            //return _streamItemFactory.Make(
            //    item,
            //    groups,
            //    "observation",
            //    groupContributionResult.GroupUser,
            //    groupContributionResult.GroupCreatedDateTime,
            //    description);
        }

        //public object MakeStreamItem(
        //    object item,
        //    IEnumerable<string> groups,
        //    string contributionType,
        //    User groupUser,
        //    DateTime groupCreatedDateTime,
        //    string description
        //)
        //{
        //    return new
        //    {
        //        CreatedDateTime = groupCreatedDateTime,
        //        CreatedDateTimeDescription = MakeCreatedDateTimeDescription(groupCreatedDateTime),
        //        Type = contributionType.ToLower(),
        //        User = new
        //        {
        //            groupUser.Id,
        //            groupUser.LastLoggedIn,
        //            Name = groupUser.FirstName + " " + groupUser.LastName,
        //            Avatar = new
        //            {
        //                AltTag = groupUser.FirstName + " " + groupUser.LastName,
        //                UrlToImage = groupUser.Avatar != null ? "" : AvatarUris.DefaultUser
        //            }
        //        },
        //        Item = item,
        //        Description = description,
        //        Groups = groups
        //    };
        //}

        //public object Make(
        //    object item,
        //    Group group,
        //    string contributionType,
        //    User groupUser,
        //    DateTime groupCreatedDateTime,
        //    string description
        //)
        //{
        //    return new
        //    {
        //        CreatedDateTime = groupCreatedDateTime,
        //        CreatedDateTimeDescription = MakeCreatedDateTimeDescription(groupCreatedDateTime),
        //        Type = contributionType.ToLower(),
        //        User = new
        //        {
        //            groupUser.Id,
        //            groupUser.LastLoggedIn,
        //            Name = groupUser.FirstName + " " + groupUser.LastName,
        //            Avatar = new
        //            {
        //                AltTag = groupUser.FirstName + " " + groupUser.LastName,
        //                UrlToImage = groupUser.Avatar != null ? "" : AvatarUris.DefaultUser
        //            }
        //        },
        //        Item = item,
        //        Description = description,
        //        Group = group
        //    };
        //}

        private static string MakeCreatedDateTimeDescription(DateTime dateTime)
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

        #endregion
    }
}