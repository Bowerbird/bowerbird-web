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
using Bowerbird.Core.Config;
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
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public SightingViewModelBuilder(
            IDocumentSession documentSession,
            ISightingViewFactory sightingViewFactory,
            ISightingNoteViewFactory sightingNoteViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _sightingViewFactory = sightingViewFactory;
            _sightingNoteViewFactory = sightingNoteViewFactory;
            _userContext = userContext;
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
            var authenticatedUser = _userContext.IsUserAuthenticated() ? _documentSession.Load<User>(_userContext.GetAuthenticatedUserId()) : null;

            var results = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ContributionId == id)
                .ToList();

            var sightingResult = results.First(x => x.ContributionType == "observation" || x.ContributionType == "record");

            var sighting = sightingResult.Contribution as Sighting;
            var projects = sightingResult.Groups;
            var user = sightingResult.User;

            dynamic viewModel = _sightingViewFactory.Make(sighting, user, projects, authenticatedUser);

            viewModel.Identifications = results.Where(x => x.ContributionType == "identification").Select(x => _sightingNoteViewFactory.Make(sighting, x.Contribution as IdentificationNew, x.User, authenticatedUser));
            viewModel.Notes = results.Where(x => x.ContributionType == "note").Select(x => _sightingNoteViewFactory.Make(sighting, x.Contribution as SightingNote, x.User, authenticatedUser));

            return viewModel;
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

            var authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            return query.Skip(queryInput.GetSkipIndex())
                .Statistics(out stats)
                .Take(queryInput.GetPageSize())
                .ToList()
                .Select(x => _sightingViewFactory.Make(x.Contribution as Sighting, x.User, x.Groups, authenticatedUser))
                .ToPagedList(
                    queryInput.Page,
                    queryInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}