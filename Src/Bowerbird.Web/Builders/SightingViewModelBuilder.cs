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
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Builders
{
    public class SightingViewModelBuilder : ISightingViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly ISightingViewFactory _sightingViewFactory;
        private readonly ISightingNoteViewFactory _sightingNoteViewFactory;

        #endregion

        #region Constructors

        public SightingViewModelBuilder(
            IDocumentSession documentSession,
            ISightingViewFactory sightingViewFactory,
            ISightingNoteViewFactory sightingNoteViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");

            _documentSession = documentSession;
            _sightingViewFactory = sightingViewFactory;
            _sightingNoteViewFactory = sightingNoteViewFactory;
        }

        #endregion

        #region Methods

        public object BuildNewObservation(string category = "", string projectId = "")
        {
            return _sightingViewFactory.MakeNewObservation(category, projectId);
        }

        public object BuildNewRecord(string projectId = null)
        {
            return _sightingViewFactory.MakeNewRecord(projectId);
        }

        public dynamic BuildSighting(string id)
        {
            var result = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ContributionId == id && (x.ContributionType == "observation" || x.ContributionType == "record" || x.ContributionType == "note"))
                .ToList();

            dynamic sighting = _sightingViewFactory.Make(result.Single(x => x.ContributionType == "observation" || x.ContributionType == "record"));

            sighting.Notes = result.Any(x => x.ContributionType == "note") ? result.Where(x => x.ContributionType == "note").Select(_sightingNoteViewFactory.Make) : new List<object>();

            return sighting;
        }

        public object BuildGroupSightingList(string groupId, SightingsQueryInput sightingsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(sightingsQueryInput, "sightingsQueryInput");

            var query = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupIds.Any(y => y == groupId) && (x.ContributionType == "observation" || x.ContributionType == "record"));

            return ExecuteQuery(sightingsQueryInput, query);

                //.Statistics(out stats)
                //.Skip(pagingInput.GetSkipIndex())
                //.Take(pagingInput.GetPageSize())
                //.ToList()
                //.Select(_sightingViewFactory.Make)
                //.ToPagedList(
                //    pagingInput.Page,
                //    pagingInput.PageSize,
                //    stats.TotalResults);
        }

        public object BuildUserSightingList(string userId, SightingsQueryInput sightingsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(sightingsQueryInput, "sightingsQueryInput");

            var query = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.UserId == userId && (x.ContributionType == "observation" || x.ContributionType == "record"));

            return ExecuteQuery(sightingsQueryInput, query);

            //.Statistics(out stats)
            //.Skip(pagingInput.GetSkipIndex())
            //.Take(pagingInput.GetPageSize())
            //.ToList()
            //.Select(_sightingViewFactory.Make)
            //.ToPagedList(
            //    pagingInput.Page,
            //    pagingInput.PageSize,
            //    stats.TotalResults);
        }

        public object BuildAllUserProjectsSightingList(string userId, SightingsQueryInput queryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(queryInput, "queryInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships.Select(x => x.Group.Id);

            var query = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupIds.Any(y => y.In(groupIds)) && (x.ContributionType == "observation" || x.ContributionType == "record"));

            return ExecuteQuery(queryInput, query);

            //switch (queryInput.Sort.ToLower())
            //{
            //    default:
            //    case "latestadded":
            //        query = query.OrderByDescending(x => x.CreatedDateTime);
            //        break;
            //    case "oldestadded":
            //        query = query.OrderBy(x => x.CreatedDateTime);
            //        break;
            //    case "a-z":
            //        query = query.OrderBy(x => x.SightingTitle);
            //        break;
            //    case "z-a":
            //        query = query.OrderByDescending(x => x.SightingTitle);
            //        break;
            //}

            //return query.Skip(queryInput.GetSkipIndex())
            //    .Take(queryInput.GetPageSize())
            //    .ToList()
            //    .Select(_sightingViewFactory.Make)
            //    .ToPagedList(
            //        queryInput.Page,
            //        queryInput.PageSize,
            //        stats.TotalResults);
        }

        private object ExecuteQuery(SightingsQueryInput queryInput, IRavenQueryable<All_Contributions.Result> query)
        {
            switch (queryInput.Sort.ToLower())
            {
                default:
                case "latestadded":
                    query = query.OrderByDescending(x => x.CreatedDateTime);
                    break;
                case "oldestadded":
                    query = query.OrderBy(x => x.CreatedDateTime);
                    break;
                case "a-z":
                    query = query.OrderBy(x => x.SightingTitle);
                    break;
                case "z-a":
                    query = query.OrderByDescending(x => x.SightingTitle);
                    break;
            }

            RavenQueryStatistics stats;

            return query.Skip(queryInput.GetSkipIndex())
                .Statistics(out stats)
                .Take(queryInput.GetPageSize())
                .ToList()
                .Select(_sightingViewFactory.Make)
                .ToPagedList(
                    queryInput.Page,
                    queryInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}