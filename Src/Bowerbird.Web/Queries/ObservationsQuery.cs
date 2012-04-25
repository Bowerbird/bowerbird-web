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

namespace Bowerbird.Web.Queries
{
    public class ObservationsQuery : IObservationsQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IObservationViewFactory _observationViewFactory;
        private readonly IStreamItemFactory _streamItemFactory;

        #endregion

        #region Constructors

        public ObservationsQuery(
            IDocumentSession documentSession,
            IObservationViewFactory observationViewFactory,
            IStreamItemFactory streamItemFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(observationViewFactory, "observationViewFactory");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");

            _documentSession = documentSession;
            _observationViewFactory = observationViewFactory;
            _streamItemFactory = streamItemFactory;
        }

        #endregion

        #region Methods

        public ObservationIndex MakeObservationIndex(IdInput idInput)
        {
            return new ObservationIndex()
            {
                Observation = _documentSession.Load<Observation>(idInput.Id)
            };
        }

        public ObservationList MakeObservationList(ObservationListInput observationListInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray();

            return new ObservationList
            {
                Page = observationListInput.Page,
                PageSize = observationListInput.PageSize,
                Observations = observations.ToPagedList(
                    observationListInput.Page,
                    observationListInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public ObservationList MakeObservationListByProjectId(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Where(x => x.Groups.Any(y => y.GroupId == listInput.GroupId))
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList();

            return new ObservationList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Project = listInput.GroupId != null ? _documentSession.Load<Project>(listInput.GroupId) : null,
                Observations = observations.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public ObservationList MakeObservationListByCreatedByUserId(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Customize(x => x.Include(listInput.CreatedByUserId))
                .Where(x => x.User.Id == listInput.CreatedByUserId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray();

            return new ObservationList
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                CreatedByUser = _documentSession.Load<User>(listInput.CreatedByUserId),
                Observations = observations.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public IEnumerable<StreamItem> MakeObservationList()
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.ContributionType.Equals("observation"))
                .OrderByDescending(x => x.CreatedDateTime)
                .Take(10)
                .ToList()
                .ToPagedList(1, 10, stats.TotalResults)
                .PagedListItems
                .Select(MakeStreamItem);
        }

        private StreamItem MakeStreamItem(All_GroupContributions.Result groupContributionResult)
        {
            object item = null;
            string description = null;
            IEnumerable<string> groups = null;

            switch (groupContributionResult.ContributionType)
            {
                case "Observation":
                    item = _observationViewFactory.Make(groupContributionResult.Observation);
                    description = groupContributionResult.Observation.User.FirstName + " added an observation";
                    groups = groupContributionResult.Observation.Groups.Select(x => x.GroupId);
                    break;
            }

            return _streamItemFactory.Make(
                item,
                groups,
                "observation",
                groupContributionResult.GroupUser,
                groupContributionResult.GroupCreatedDateTime,
                description);
        }


        #endregion
    }
}