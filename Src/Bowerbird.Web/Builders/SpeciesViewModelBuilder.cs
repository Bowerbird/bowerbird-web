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
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Paging;

namespace Bowerbird.Web.Builders
{
    public class SpeciesViewModelBuilder : ISpeciesViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly ISpeciesViewFactory _speciesViewFactory;

        #endregion

        #region Constructors

        public SpeciesViewModelBuilder(
            IDocumentSession documentSession,
            ISpeciesViewFactory speciesViewFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(speciesViewFactory, "speciesViewFactory");

            _documentSession = documentSession;
            _speciesViewFactory = speciesViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildItem(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _speciesViewFactory.Make(_documentSession.Load<Species>(idInput.Id));
        }

        public object BuildList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            if (pagingInput.Id != null)
            {
                return BuildGroupSpecies(pagingInput);
            }

            return BuildSpecies(pagingInput);
        }

        private object BuildSpecies(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Species>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => new
                {
                    x.Id,
                    x.CommonNames,
                    x.Family,
                    x.GenusName,
                    x.Group,
                    x.Kingdom,
                    x.Order,
                    x.SpeciesName,
                    x.Taxonomy
                });

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                SpeciesList = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object BuildGroupSpecies(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Species>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => new
                {
                    x.Id,
                    x.CommonNames,
                    x.Family,
                    x.GenusName,
                    x.Group,
                    x.Kingdom,
                    x.Order,
                    x.SpeciesName,
                    x.Taxonomy
                });

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                SpeciesList = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}