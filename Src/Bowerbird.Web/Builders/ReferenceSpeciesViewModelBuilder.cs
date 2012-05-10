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
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Builders
{
    public class ReferenceSpeciesViewModelBuilder : IReferenceSpeciesViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IReferenceSpeciesViewFactory _referenceSpeciesViewFactory;

        #endregion

        #region Constructors

        public ReferenceSpeciesViewModelBuilder(
            IDocumentSession documentSession,
            IReferenceSpeciesViewFactory referenceSpeciesViewFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(referenceSpeciesViewFactory, "referenceSpeciesViewFactory");

            _documentSession = documentSession;
            _referenceSpeciesViewFactory = referenceSpeciesViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildReferenceSpecies(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _referenceSpeciesViewFactory.Make(_documentSession.Load<ReferenceSpecies>(idInput.Id));
        }

        public object BuildReferenceSpeciesList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_ReferenceSpecies.Result, All_ReferenceSpecies>()
                .AsProjection<All_ReferenceSpecies.Result>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _referenceSpeciesViewFactory.Make(x.ReferenceSpecies));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Organisations = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        public object BuildGroupReferenceSpeciesList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<All_ReferenceSpecies.Result, All_ReferenceSpecies>()
                .AsProjection<All_ReferenceSpecies.Result>()
                .Where(x => x.GroupId == pagingInput.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _referenceSpeciesViewFactory.Make(x.ReferenceSpecies));

            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Organisations = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}