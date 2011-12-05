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
    public class HomeIndexFactory : ViewModelFactoryBase<HomeIndexInput, HomeIndex>
    {

        #region Members

        private readonly IPagedListFactory _pagedListFactory;

        #endregion

        #region Constructors

        public HomeIndexFactory(
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

        public override HomeIndex Make(HomeIndexInput homeIndexInput)
        {
            RavenQueryStatistics stats;

            //int requestedPageSize = pageSize < 1 || pageSize > 30 ? 10 : pageSize;
            //int requestedPage = page < 1 ? 1 : page;

            var results = DocumentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Where(x => x.User.Id == homeIndexInput.Username)
                .Skip(homeIndexInput.Page)
                .Take(homeIndexInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new HomeIndex()
            {
                Observations = _pagedListFactory.Make(homeIndexInput.Page, homeIndexInput.PageSize, stats.TotalResults, results, null)
            };
        }

        #endregion      
      
    }
}
