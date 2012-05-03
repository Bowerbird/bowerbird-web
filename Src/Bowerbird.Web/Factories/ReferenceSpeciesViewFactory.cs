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
using Raven.Client;

namespace Bowerbird.Web.Factories
{
    public class ReferenceSpeciesViewFactory : IReferenceSpeciesViewFactory
    {
        #region Fields

        private readonly IUserViewFactory _userViewFactory;
        private readonly ISpeciesViewFactory _speciesViewFactory;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ReferenceSpeciesViewFactory(
            IUserViewFactory userViewFactory,
            ISpeciesViewFactory speciesViewFactory,
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(speciesViewFactory, "speciesViewFactory");
            Check.RequireNotNull(documentSession, "documentSession");

            _userViewFactory = userViewFactory;
            _speciesViewFactory = speciesViewFactory;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(ReferenceSpecies referenceSpecies)
        {
            return new
            {
                referenceSpecies.Id,
                referenceSpecies.CreatedDateTime,
                referenceSpecies.GroupId,
                referenceSpecies.SmartTags,
                Creator = _userViewFactory.Make(referenceSpecies.User.Id),
                Species = _speciesViewFactory.Make(_documentSession.Load<Species>(referenceSpecies.SpeciesId))
            };
        }

        #endregion
    }
}