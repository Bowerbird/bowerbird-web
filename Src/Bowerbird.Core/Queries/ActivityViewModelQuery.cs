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
using Bowerbird.Core.ViewModelFactories;
using Bowerbird.Core.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Paging;
using System.Dynamic;
using StackExchange.Profiling;

namespace Bowerbird.Core.Queries
{
    public class ActivityViewModelQuery : IActivityViewModelQuery
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
        private readonly IIdentificationViewFactory _identificationViewFactory;
        private readonly IPostViewFactory _postViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ActivityViewModelQuery(
            IDocumentSession documentSession,
            ISightingViewFactory sightingViewFactory,
            ISightingNoteViewFactory sightingNoteViewFactory,
            IIdentificationViewFactory identificationViewFactory,
            IPostViewFactory postViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");
            Check.RequireNotNull(identificationViewFactory, "identificationViewFactory");
            Check.RequireNotNull(postViewFactory, "postViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _sightingViewFactory = sightingViewFactory;
            _sightingNoteViewFactory = sightingNoteViewFactory;
            _identificationViewFactory = identificationViewFactory;
            _postViewFactory = postViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildHomeActivityList(string userId, ActivitiesQueryInput activityInput, PagingInput pagingInput)
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

            return Execute(query, activityInput, pagingInput);
        }

        public object BuildUserActivityList(string userId, ActivitiesQueryInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.UserId == userId && x.Type.In(_activityTypes));

            return Execute(query, activityInput, pagingInput);
        }

        public object BuildGroupActivityList(string groupId, ActivitiesQueryInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.GroupIds.Any(y => y == groupId) && x.Type.In(_activityTypes));

            return Execute(query, activityInput, pagingInput);
        }

        public object BuildNotificationActivityList(string userId, ActivitiesQueryInput activityInput, PagingInput pagingInput)
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

            return Execute(query, activityInput, pagingInput);
        }

        private object Execute(IRavenQueryable<All_Activities.Result> query, ActivitiesQueryInput activityInput, PagingInput pagingInput)
        {
            var profiler = MiniProfiler.Current;

            if (activityInput.NewerThan.HasValue)
            {
                query = query.Where(x => x.CreatedDateTime > activityInput.NewerThan.Value);
            }
            else if (activityInput.OlderThan.HasValue)
            {
                query = query.Where(x => x.CreatedDateTime < activityInput.OlderThan.Value);
            }

            RavenQueryStatistics stats;

            List<dynamic> activities;

            using (profiler.Step("Get top 10 activity list RavenDB query"))
            {
                activities = query
                    .Statistics(out stats)
                    .OrderByDescending(x => x.CreatedDateTime)
                    .Skip(pagingInput.GetSkipIndex())
                    .Take(pagingInput.GetPageSize())
                    .As<dynamic>()
                    .ToList();
            }


            var contributionIds = activities.SelectMany(x => new[] { x.ContributionId, x.SubContributionId }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();

            List<All_Contributions.Result> contributions;

            using (profiler.Step("Get the 10 contributions RavenDB query"))
            {
                contributions = _documentSession
                    .Query<All_Contributions.Result, All_Contributions>()
                    .Where(x => x.ParentContributionId.In(contributionIds))
                    .ToList();
            }

            User authenticatedUser = null;

            using (profiler.Step("Get authenticated user RavenDB query"))
            {
                if (_userContext.IsUserAuthenticated())
                {
                    authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
                }
            }

            using (profiler.Step("Build activity list view model"))
            {
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

                            if (result == null)
                            {
                                activity.DeletedActivityItem = MakeDeletedActivityItem("sighting");
                            }
                            else
                            {
                                activity.ObservationAdded = new
                                {
                                    Observation = _sightingViewFactory.Make(result.Contribution as Sighting, result.User, result.Groups, authenticatedUser)
                                };
                            }

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

                            if (result == null)
                            {
                                activity.DeletedActivityItem = MakeDeletedActivityItem("identification");
                            }
                            else
                            {
                                activity.IdentificationAdded = new
                                {
                                    Sighting = _sightingViewFactory.Make(result.ParentContribution as Sighting, result.User, result.Groups, authenticatedUser),
                                    Identification = _identificationViewFactory.Make(result.ParentContribution as Sighting, result.Contribution as IdentificationNew, result.User, authenticatedUser)
                                };
                            }
                        }

                        if (x.Type == "sightingnoteadded")
                        {
                            var result = contributions.FirstOrDefault(y => y.ParentContributionId == x.ContributionId && y.SubContributionId == x.SubContributionId && y.SubContributionType == "note");

                            if (result == null)
                            {
                                activity.DeletedActivityItem = MakeDeletedActivityItem("note");
                            }
                            else
                            {
                                activity.SightingNoteAdded = new
                                {
                                    Sighting = _sightingViewFactory.Make(result.ParentContribution as Sighting, result.User, result.Groups, authenticatedUser),
                                    SightingNote = _sightingNoteViewFactory.Make(result.ParentContribution as Sighting, result.Contribution as SightingNote, result.User, authenticatedUser)
                                };
                            }
                        }

                        if (x.Type == "postadded")
                        {
                            var result = contributions.FirstOrDefault(y => y.ParentContributionId == x.ContributionId && y.ParentContributionType == "post");

                            if (result == null)
                            {
                                activity.DeletedActivityItem = MakeDeletedActivityItem("news item");
                            }
                            else
                            {
                                activity.PostAdded = new
                                {
                                    Post = _postViewFactory.Make(result.Contribution as Post, result.User, result.Groups.First(), authenticatedUser)
                                };
                            }
                        }

                        return activity;
                    })
                };
            }
        }

        private object MakeDeletedActivityItem(string type)
        {
            return new
            {
                Message = string.Format("This {0} has been removed.", type)
            };
        }

        #endregion
    }
}