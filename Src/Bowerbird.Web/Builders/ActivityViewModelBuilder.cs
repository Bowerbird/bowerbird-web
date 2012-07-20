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

namespace Bowerbird.Web.Builders
{
    public class ActivityViewModelBuilder : IActivityViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IEnumerable<string> _activityTypes = new[] 
            {
                "observationadded",
                "postadded",
                "observationnoteadded"
            };

        #endregion

        #region Constructors

        public ActivityViewModelBuilder(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
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

            return RunQuery(query, activityInput, pagingInput);
        }

        public object BuildUserActivityList(string userId, ActivityInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.UserId == userId && x.Type.In(_activityTypes));

            return RunQuery(query, activityInput, pagingInput);
        }

        public object BuildGroupActivityList(string groupId, ActivityInput activityInput, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(activityInput, "activityInput");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Activities.Result>("All/Activities")
                .Where(x => x.GroupIds.Any(y => y == groupId) && x.Type.In(_activityTypes));

            return RunQuery(query, activityInput, pagingInput);
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

            return RunQuery(query, activityInput, pagingInput);
        }

        private object RunQuery(IRavenQueryable<All_Activities.Result> query, ActivityInput activityInput, PagingInput pagingInput)
        {
            if (activityInput.NewerThan.HasValue)
            {
                query.Where(x => x.CreatedDateTime > activityInput.NewerThan.Value);
            }
            else if (activityInput.OlderThan.HasValue)
            {
                query.Where(x => x.CreatedDateTime < activityInput.OlderThan.Value);
            }

            RavenQueryStatistics stats;

            return query
                .Statistics(out stats)
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

        #endregion
    }
}