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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Paging;
using System.Dynamic;

namespace Bowerbird.Web.Builders
{
    public class ActivityViewModelBuilder : IActivityViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IEnumerable<string> _activityTypes = new[] 
            {
                "sightingadded",
                "identificationadded",
                "sightingnoteadded",
                "postadded"
            };

        private readonly ISightingViewFactory _sightingViewFactory;
        private readonly ISightingNoteViewFactory _sightingNoteViewFactory;
        private readonly IPostViewFactory _postViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ActivityViewModelBuilder(
            IDocumentSession documentSession,
            ISightingViewFactory sightingViewFactory,
            ISightingNoteViewFactory sightingNoteViewFactory,
            IPostViewFactory postViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");
            Check.RequireNotNull(postViewFactory, "postViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _sightingViewFactory = sightingViewFactory;
            _sightingNoteViewFactory = sightingNoteViewFactory;
            _postViewFactory = postViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildHomeActivityList(string userId, ActivityInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships.Select(x => x.Group.Id);

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.GroupIds.Any(y => y.In(groupIds)) && x.Type.In(_activityTypes));

            return RunQuery2(query, activityInput, pagingInput);
        }

        public object BuildUserActivityList(string userId, ActivityInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.UserId == userId && x.Type.In(_activityTypes));

            return RunQuery2(query, activityInput, pagingInput);
        }

        public object BuildGroupActivityList(string groupId, ActivityInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.GroupIds.Any(y => y == groupId) && x.Type.In(_activityTypes));

            return RunQuery2(query, activityInput, pagingInput);
        }

        public object BuildNotificationActivityList(string userId, ActivityInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships.Select(x => x.Group.Id);

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.GroupIds.Any(y => y.In(groupIds)));

            return RunQuery2(query, activityInput, pagingInput);
        }

        private object RunQuery2(IRavenQueryable<All_Activities.Result> query, ActivityInput activityInput, PagingInput pagingInput)
        {
            if (activityInput.NewerThan.HasValue)
            {
                query = query.Where(x => x.CreatedDateTime > activityInput.NewerThan.Value);
            }
            else if (activityInput.OlderThan.HasValue)
            {
                query = query.Where(x => x.CreatedDateTime < activityInput.OlderThan.Value);
            }

            RavenQueryStatistics stats;

            var activities = query
                .Statistics(out stats)
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.GetPageSize())
                .As<dynamic>()
                .ToList();

            var contributionIds = activities.SelectMany(x => new[] { x.ContributionId, x.SubContributionId }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();

            var contributions = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .Where(x => x.ParentContributionId.In(contributionIds))
                .ToList();

            var authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            return new PagedList<object>()
            {
                Page = pagingInput.Page,
                PageSize = pagingInput.PageSize,
                TotalResultCount = stats.TotalResults,
                PagedListItems = activities.Select(x =>
                {
                    dynamic activity = new ExpandoObject();

                    activity.Id = x.Id;
                    activity.Type = x.Type;
                    activity.CreatedDateTime = x.CreatedDateTime;
                    activity.CreatedDateTimeOrder = x.CreatedDateTimeOrder;
                    activity.Description = x.Description;
                    activity.User = x.User;
                    activity.Groups = x.Groups;
                    activity.ContributionId = x.ContributionId;
                    activity.SubContributionId = x.SubContributionId;

                    if (x.Type == "sightingadded")
                    {
                        var result = contributions.FirstOrDefault(y => y.ParentContributionId == x.ContributionId && (y.ParentContributionType == "observation" || y.ParentContributionType == "record"));
                        object sighting = null;

                        if (result != null)
                        {
                            sighting = _sightingViewFactory.Make(result.Contribution as Sighting, result.User, result.Groups, authenticatedUser);
                        }

                        activity.ObservationAdded = new
                        {
                            Observation = sighting
                        };
                        //if (x.RecordAdded != null)
                        //{
                        //    activity.RecordAdded = new
                        //        {
                        //            Record = sighting
                        //        };
                        //}
                    }

                    if (x.Type == "identificationadded")
                    {
                        var result = contributions.FirstOrDefault(y => y.ParentContributionId == x.ContributionId && y.SubContributionId == x.SubContributionId && y.SubContributionType == "identification");
                        object sighting = null;
                        object identitification = null;

                        if (result != null)
                        {
                            sighting = _sightingViewFactory.Make(result.ParentContribution as Sighting, result.User, result.Groups, authenticatedUser);
                            identitification = _sightingNoteViewFactory.Make(result.ParentContribution as Sighting, result.Contribution as IdentificationNew, result.User, authenticatedUser);
                        }

                        activity.IdentificationAdded = new
                        {
                            Sighting = sighting,
                            Identification = identitification
                        };
                    }

                    if (x.Type == "sightingnoteadded")
                    {
                        var result = contributions.FirstOrDefault(y => y.ParentContributionId == x.ContributionId && y.SubContributionId == x.SubContributionId && y.SubContributionType == "note");
                        object sighting = null;
                        object sightingNote = null;

                        if (result != null)
                        {
                            sighting = _sightingViewFactory.Make(result.ParentContribution as Sighting, result.User, result.Groups, authenticatedUser);
                            sightingNote = _sightingNoteViewFactory.Make(result.ParentContribution as Sighting, result.Contribution as SightingNote, result.User, authenticatedUser);
                        }

                        activity.SightingNoteAdded = new
                        {
                            Sighting = sighting,
                            SightingNote = sightingNote
                        };
                    }

                    if (x.Type == "postadded")
                    {
                        var result = contributions.FirstOrDefault(y => y.ParentContributionId == x.ContributionId && y.ParentContributionType == "post");
                        object post = null;

                        if (result != null)
                        {
                            post = _postViewFactory.Make(result.Contribution as Post, result.User, result.Groups.First(), authenticatedUser);
                        }

                        activity.PostAdded = new
                        {
                            Post = post
                        };
                    }

                    return activity;
                })
            };
        }

        #endregion
    }
}