using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Web.ViewModels.Members;
using Raven.Client;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels;
using Raven.Client.Linq;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.ViewModelFactories
{
    public class ObservationListFactory : ViewModelFactoryBase, IViewModelFactory<ObservationListInput, ObservationList>
    {

        #region Members

        private readonly IPagedListFactory _pagedListFactory;

        #endregion

        #region Constructors

        public ObservationListFactory(
            IDocumentSession documentSession,
            IPagedListFactory pagedListFactory)
            : base(documentSession)
        {
            Check.RequireNotNull(pagedListFactory, "pagedListFactory");

            _pagedListFactory = pagedListFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ObservationList Make(ObservationListInput observationListInput)
        {
            Check.RequireNotNull(observationListInput, "observationListInput");

            RavenQueryStatistics stats;

            var results = DocumentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Where(x => x.User.Id == observationListInput.UserId)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray();
                // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new ObservationList
                       {
                           UserId = observationListInput.UserId,
                           ProjectId = observationListInput.ProjectId,
                           TeamId = observationListInput.TeamId,
                           Page = observationListInput.Page,
                           PageSize = observationListInput.PageSize,
                           Observations = _pagedListFactory.Make(
                               observationListInput.Page,
                               observationListInput.PageSize,
                               stats.TotalResults,
                               results,
                               null)
                       };
        }

        #endregion

    }
}
