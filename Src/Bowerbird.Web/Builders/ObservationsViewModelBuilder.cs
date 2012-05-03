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

        public object BuildList(PagingInput pagingInput)
        {
            //if (pagingInput.Id != null)
            //{
            //    return MakeObservationListByProjectId(pagingInput);
            //}

            //if (pagingInput.CreatedByUserId != null)
            //{
            //    return MakeObservationListByCreatedByUserId(pagingInput);
            //}

            //return MakeObservationList(pagingInput);
            throw new System.NotImplementedException();
        }

        public object BuildStreamItems(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_GroupContributions.Result, All_GroupContributions>()
                .AsProjection<All_GroupContributions.Result>()
                .Statistics(out stats)
                .Include(x => x.ContributionId)
                .Where(x => x.ContributionType.Equals("observation"))
                .OrderByDescending(x => x.CreatedDateTime)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .ToPagedList(pagingInput.Page, pagingInput.PageSize, stats.TotalResults)
                .PagedListItems
                .Select(MakeStreamItem);
        }

        private object MakeObservationList(PagingInput pagingInput)
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

        private object MakeObservationListByProjectId(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var observations = _documentSession
                .Query<Observation>()
                .Where(x => x.Groups.Any(y => y.GroupId == pagingInput.Id))
                .Statistics(out stats)
                .Skip(pagingInput.Page.Or(Default.PageStart))
                .Take(pagingInput.PageSize.Or(Default.PageSize))
                .ToList();

            return new
            {
                Page = pagingInput.Page.Or(Default.PageStart),
                PageSize = pagingInput.PageSize.Or(Default.PageSize),
                Project = pagingInput.Id != null ? _documentSession.Load<Project>(pagingInput.Id) : null,
                Observations = observations.ToPagedList(
                    pagingInput.Page.Or(Default.PageStart),
                    pagingInput.PageSize.Or(Default.PageSize),
                    stats.TotalResults,
                    null)
            };
        }

        private object MakeObservationListByCreatedByUserId(PagingInput pagingInput)
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