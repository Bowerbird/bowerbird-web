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

        #endregion

        #region Constructors

        public SpeciesViewModelBuilder(
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildSpecies(string speciesId)
        {
            Check.RequireNotNull(speciesId, "speciesId");

            return MakeSpecies(_documentSession.Load<Species>(speciesId));
        }

        public object BuildSpeciesList(SpeciesQueryInput query, PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var aaa = _documentSession
                .Query<All_Species.Result, All_Species>()
                .Statistics(out stats)
                //.Where(x => x.QueryField == query.Query)
                .Take(20)
                .As<Species>()
                .ToList();
            //.Select(MakeSpecies);


            return aaa;

            //RavenQueryStatistics stats;

            //return _documentSession
            //    .Query<Species>()
            //    .Statistics(out stats)
            //    .Skip(pagingInput.GetSkipIndex())
            //    .Take(pagingInput.PageSize)
            //    .ToList()
            //    .Select(MakeSpecies)
            //    .ToPagedList(
            //        pagingInput.Page,
            //        pagingInput.PageSize,
            //        stats.TotalResults,
            //        null);
        }

        private static object MakeSpecies(Species species)
        {
            return new
            {
                species.Id,
                species.CommonGroupName,
                species.CommonNames,
                species.KingdomName,
                species.PhylumName,
                species.ClassName,
                species.OrderName,
                species.FamilyName,
                species.GenusName,
                species.SpeciesName
            };
        }

        #endregion
    }
}