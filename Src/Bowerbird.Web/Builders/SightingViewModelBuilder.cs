/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.DesignByContract;
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

        #endregion

        #region Constructors

        public SightingViewModelBuilder(
            IDocumentSession documentSession,
            ISightingViewFactory sightingViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");

            _documentSession = documentSession;
            _sightingViewFactory = sightingViewFactory;
        }

        #endregion

        #region Methods

        public object BuildNewObservation(string projectId = null)
        {
            return _sightingViewFactory.MakeNewObservation(projectId);
        }

        public object BuildNewRecord(string projectId = null)
        {
            return _sightingViewFactory.MakeNewRecord(projectId);
        }

        public object BuildSighting(string id)
        {
            var result = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ContributionId == id && (x.ContributionType == "observation" || x.ContributionType == "record"))
                .ToList()
                .First();

            return _sightingViewFactory.Make(result);
        }

        public object BuildGroupSightingList(string groupId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupIds.Any(y => y == groupId) && (x.ContributionType == "observation" || x.ContributionType == "record"))
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_sightingViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        public object BuildUserSightingList(string userId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.UserId == userId && (x.ContributionType == "observation" || x.ContributionType == "record"))
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_sightingViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}