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
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Builders
{
    public class ObservationsViewModelBuilder : IObservationsViewModelBuilder
    {
        #region Fields

        private readonly IObservationViewFactory _observationViewFactory;
        private readonly IStreamItemFactory _streamItemFactory;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ObservationsViewModelBuilder(
            IObservationViewFactory observationViewFactory,
            IStreamItemFactory streamItemFactory,
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(observationViewFactory, "observationViewFactory");
            Check.RequireNotNull(streamItemFactory, "streamItemFactory");
            Check.RequireNotNull(documentSession, "documentSession");

            _observationViewFactory = observationViewFactory;
            _streamItemFactory = streamItemFactory;
            _documentSession = documentSession;
        }

        #endregion

        #region Methods

        public object BuildItem(IdInput idInput)
        {
            var observation = _documentSession.Load<Observation>(idInput.Id);

            return new 
            {
                observation.Address,
                observation.Id,
                observation.IsIdentificationRequired,
                observation.Latitude,
                observation.Longitude,
                observation.ObservationCategory,
                //ObservationMedia = {},
                observation.ObservedOn,
                observation.Title,
                Projects = observation.Groups.Select(x => x.GroupId)
            };
        }

        public object BuildList(ObservationListInput listInput)
        {
            if (listInput.GroupId != null)
            {
                return MakeObservationListByProjectId(listInput);
            }

            if (listInput.CreatedByUserId != null)
            {
                return MakeObservationListByCreatedByUserId(listInput);
            }

            return MakeObservationList(listInput);
        }

        public object BuildStreamItems(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.ContributionType.Equals("observation"))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .ToPagedList(listInput.Page, listInput.PageSize, stats.TotalResults)
                .PagedListItems
                .Select(MakeStreamItem);
        }

        private object MakeObservationList(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray();

            return new
            {
                listInput.Page,
                listInput.PageSize,
                Observations = observations.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object MakeObservationListByProjectId(ObservationListInput listInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Where(x => x.Groups.Any(y => y.GroupId == listInput.GroupId))
                .Statistics(out stats)
                .Skip(listInput.Page.Or(Default.PageStart))
                .Take(listInput.PageSize.Or(Default.PageSize))
                .ToList();

            return new
            {
                Page = listInput.Page.Or(Default.PageStart),
                PageSize = listInput.PageSize.Or(Default.PageSize),
                Project = listInput.GroupId != null ? _documentSession.Load<Project>(listInput.GroupId) : null,
                Observations = observations.ToPagedList(
                    listInput.Page.Or(Default.PageStart),
                    listInput.PageSize.Or(Default.PageSize),
                    stats.TotalResults,
                    null)
            };
        }

        private object MakeObservationListByCreatedByUserId(ObservationListInput listInput)
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

            return new
            {
                listInput.Page,
                listInput.PageSize,
                CreatedByUser = _documentSession.Load<User>(listInput.CreatedByUserId),
                Observations = observations.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object MakeStreamItem(All_GroupContributions.Result groupContributionResult)
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