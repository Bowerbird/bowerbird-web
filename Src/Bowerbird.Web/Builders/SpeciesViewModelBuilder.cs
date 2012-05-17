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
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public SpeciesViewModelBuilder(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory
        )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildSpecies(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return MakeSpecies(_documentSession.Load<Species>(idInput.Id));
        }

        public object BuildSpeciesList(PagingInput pagingInput)
        {
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Species>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeSpecies);

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

        private object MakeSpecies(Species species)
        {
            return new
            {
                species.Id,
                species.Kingdom,
                species.Order,
                species.Group,
                species.SpeciesName,
                species.Taxonomy,
                species.GenusName,
                species.Family,
                species.CommonNames
                //,
                //species.ProposedAsNewSpecies,
                //ProposedBy = species.ProposedAsNewSpecies ? MakeUser(species.ProposedByUser.Id) : null,
                //EndorsedBy = species.ProposedAsNewSpecies ? MakeUser(species.EndorsedByUser.Id) : null,
                //Creator = MakeUser(species.CreatedByUser.Id)
            };
        }

        #endregion
    }
}