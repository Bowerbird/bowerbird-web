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

        public object BuildSighting(string id)
        {
            var authenticatedUser = _userContext.IsUserAuthenticated() ? _documentSession.Load<User>(_userContext.GetAuthenticatedUserId()) : null;

            var results = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ParentContributionId == id)
                .ToList();

            var sightingResult = results.First(x => (x.ParentContributionType == "observation" || x.ParentContributionType == "record") && x.SubContributionType == null);

            var sighting = sightingResult.Contribution as Sighting;
            var projects = sightingResult.Groups;
            var user = sightingResult.User;

            dynamic viewModel = _sightingViewFactory.Make(sighting, user, projects, authenticatedUser);

            viewModel.Identifications = results.Where(x => x.SubContributionType == "identification").Select(x => _sightingNoteViewFactory.Make(sighting, x.Contribution as IdentificationNew, x.User, authenticatedUser));
            viewModel.Notes = results.Where(x => x.SubContributionType == "note").Select(x => _sightingNoteViewFactory.Make(sighting, x.Contribution as SightingNote, x.User, authenticatedUser));

            return viewModel;
        }

        public object BuildSightingList(SightingsQueryInput sightingsQueryInput)
        {
            Check.RequireNotNull(sightingsQueryInput, "sightingsQueryInput");

            return ExecuteQuery(sightingsQueryInput, new string[] { });
        }

        public object BuildGroupSightingList(string groupId, SightingsQueryInput sightingsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(sightingsQueryInput, "sightingsQueryInput");

            return ExecuteQuery(sightingsQueryInput, new[] { groupId });
        }

        public object BuildFavouritesSightingList(string userId, SightingsQueryInput sightingsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(sightingsQueryInput, "sightingsQueryInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships
                .Where(x => x.Group.GroupType == "favourites")
                .Select(x => x.Group.Id);

            return ExecuteQuery(sightingsQueryInput, groupIds);
        }

        public object BuildUserSightingList(string userId, SightingsQueryInput sightingsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(sightingsQueryInput, "sightingsQueryInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships
                .Where(x => x.Group.GroupType == "userproject")
                .Select(x => x.Group.Id);

            return ExecuteQuery(sightingsQueryInput, groupIds);
        }

        public object BuildHomeSightingList(string userId, SightingsQueryInput sightingsQueryInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(sightingsQueryInput, "sightingsQueryInput");

            var groupIds = _documentSession
                .Load<User>(userId)
                .Memberships.Select(x => x.Group.Id);

            return ExecuteQuery(sightingsQueryInput, groupIds);
        }

        private object ExecuteQuery(SightingsQueryInput sightingsQueryInput, IEnumerable<string> groupIds)
        {
            RavenQueryStatistics stats;
            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Contributions.Result, All_Contributions>()
                .Statistics(out stats)
                .SelectFields<All_Contributions.Result>("GroupIds", "CreatedDateTime", "ParentContributionId", "SubContributionId", "ParentContributionType", "SubContributionType", "UserId", "Observation", "Record", "Post", "User")
                .WhereIn("ParentContributionType", new[] { "observation", "record" })
                .AndAlso()
                .WhereEquals("SubContributionType", null);

            if (groupIds.Any())
            {
                query = query
                    .AndAlso()
                    .WhereIn("GroupIds", groupIds);
            }

            if (!string.IsNullOrWhiteSpace(sightingsQueryInput.Category))
            {
                query = query
                    .AndAlso()
                    .WhereEquals("SightingCategory", sightingsQueryInput.Category);
            }

            if (sightingsQueryInput.NeedsId)
            {
                query = query
                    .AndAlso()
                    .WhereEquals("SightingIdentificationCount", 0);
            }

            if (!string.IsNullOrWhiteSpace(sightingsQueryInput.Query))
            {
                var field = "SightingAllFields";

                if (sightingsQueryInput.Field.ToLower() == "title")
                {
                    field = "SightingTitle";
                }
                if (sightingsQueryInput.Field.ToLower() == "descriptions")
                {
                    field = "SightingDescriptions";
                }
                if (sightingsQueryInput.Field.ToLower() == "tags")
                {
                    field = "SightingTags";
                }

                query = query
                    .AndAlso()
                    .Search(field, sightingsQueryInput.Query);
            }

            if (!string.IsNullOrWhiteSpace(sightingsQueryInput.Taxonomy))
            {
                var ranks = sightingsQueryInput.Taxonomy.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rank in ranks)
                {
                    query = query
                        .AndAlso()
                        .Search("SightingTaxonomicRanks", rank);
                }
            }

            switch (sightingsQueryInput.Sort.ToLower())
            {
                default:
                case "newest":
                    query = query.AddOrder(x => x.CreatedDateTime, true).AddOrder(x => x.SightingTitle, false);
                    break;
                case "oldest":
                    query = query.AddOrder(x => x.CreatedDateTime, false).AddOrder(x => x.SightingTitle, false);
                    break;
                case "a-z":
                    query = query.AddOrder(x => x.SightingTitle, false);
                    break;
                case "z-a":
                    query = query.AddOrder(x => x.SightingTitle, true);
                    break;
                //case "popular": // Most popular
                // break;
                //case "active": // Having most activity
                // break;
                //case "needsid": // Needs an identification
                // break;
            }

            return query
                .Skip(sightingsQueryInput.GetSkipIndex())
                .Take(sightingsQueryInput.GetPageSize())
                .ToList()
                .Select(x => _sightingViewFactory.Make(x.Contribution as Sighting, x.User, x.Groups, authenticatedUser))
                .ToPagedList(
                    sightingsQueryInput.GetPage(),
                    sightingsQueryInput.GetPageSize(),
                    stats.TotalResults
                );
        }

        #endregion
    }
}