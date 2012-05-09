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

        public object BuildObservation(IdInput idInput)
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

        public object BuildObservationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToArray();

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Observations = observations.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }
        
        public object BuildProjectObservationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Where(x => x.Groups.Any(y => y.GroupId == pagingInput.Id))
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList();

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Project = pagingInput.Id != null ? _documentSession.Load<Project>(pagingInput.Id) : null,
                Observations = observations.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildUserObservationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Customize(x => x.Include(pagingInput.Id))
                .Where(x => x.User.Id == pagingInput.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToArray();

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                CreatedByUser = _documentSession.Load<User>(pagingInput.Id),
                Observations = observations.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildObservationStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.ContributionType.Equals("observation") && x.GroupId == pagingInput.Id)
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .ToPagedList(pagingInput.Page, pagingInput.PageSize, stats.TotalResults)
                .PagedListItems
                .Select(MakeStreamItem);
        }

        private object MakeStreamItem(All_Contributions.Result groupContributionResult)
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