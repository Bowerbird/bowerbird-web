/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Factories
{
    public class SpeciesViewFactory : ISpeciesViewFactory
    {
        #region Fields

        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public SpeciesViewFactory(
            IUserViewFactory userViewFactory
            )
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(Species species)
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
                species.CommonNames,
                species.ProposedAsNewSpecies,
                ProposedBy = species.ProposedAsNewSpecies ? _userViewFactory.Make(species.ProposedByUser.Id) : null,
                EndorsedBy = species.ProposedAsNewSpecies ? _userViewFactory.Make(species.EndorsedByUser.Id) : null,
                Creator = _userViewFactory.Make(species.CreatedByUser.Id)
            };
        }

        #endregion
    }
}