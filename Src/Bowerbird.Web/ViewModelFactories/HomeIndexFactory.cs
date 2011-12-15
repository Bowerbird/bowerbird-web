/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Web.ViewModelFactories
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Raven.Client;
    using Raven.Client.Linq;

    using Bowerbird.Core.Entities;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Core.DesignByContract;

    #endregion

    public class HomeIndexFactory : ViewModelFactoryBase<HomeIndexInput, HomeIndex>
    {

        #region Fields

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

            var streamItems = new List<StreamItem>();

            // Observations
            streamItems.AddRange(DocumentSession
                .Query<Observation>()
                .Statistics(out stats)
                //.Where(x => x.Teams.In(subscription.Teams) || x.Projects.In(subscription.Projects) || x.User.Id == homeIndexInput.Username)
                .Where(x => x.User.Id == homeIndexInput.Username)
                .OrderByDescending(x => x.SubmittedOn)
                .Skip(homeIndexInput.Page)
                .Take(homeIndexInput.PageSize)
                .Select(x => new StreamItem() { Type = "observation", SubmittedOn = x.SubmittedOn, Item = x }) // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
                .ToList());

            // Posts
            streamItems.AddRange(DocumentSession
                .Query<Post>()
                .OrderByDescending(x => x.PostedOn)
                .Skip(homeIndexInput.Page)
                .Take(homeIndexInput.PageSize)
                .Select(x => new StreamItem() { Type = "post", SubmittedOn = x.PostedOn, Item = x }) // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds
                .ToList());

            // Get number required based on page size
            streamItems = streamItems
                .OrderByDescending(x => x.SubmittedOn)
                .Take(homeIndexInput.PageSize)
                .ToList();

            return new HomeIndex()
            {
                StreamItems = _pagedListFactory.Make(homeIndexInput.Page, homeIndexInput.PageSize, stats.TotalResults, streamItems, null)
            };
        }

        #endregion      
      
    }
}