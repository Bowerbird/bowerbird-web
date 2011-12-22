using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Bowerbird.Core.Entities;
using Bowerbird.Web.ViewModels;
using Raven.Client.Linq;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.ViewModelFactories
{
    public class UserObservationsFactory : ViewModelFactoryBase<ObservationListInput, PagedList<Observation>>
    {

        #region Members

        private readonly IPagedListFactory _pagedListFactory;

        #endregion

        #region Constructors

        public UserObservationsFactory(
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

        public override PagedList<Observation> Make(ObservationListInput observationListInput)
        {
            Check.RequireNotNull(observationListInput, "observationListInput");

            RavenQueryStatistics stats;

            var results = DocumentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Where(x => x.User.Id == observationListInput.UserId)
                .Skip(observationListInput.Page)
                .Take(observationListInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return _pagedListFactory.Make(
                observationListInput.Page, 
                observationListInput.PageSize, 
                stats.TotalResults, 
                results, 
                null);
        }

        #endregion      
      
    }
}
