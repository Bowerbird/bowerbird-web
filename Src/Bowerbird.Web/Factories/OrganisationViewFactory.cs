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
    public class OrganisationViewFactory : IOrganisationViewFactory
    {
        #region Fields

        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public OrganisationViewFactory(
            IAvatarFactory avatarFactory
            )
        {
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(Organisation organisation)
        {
            Check.RequireNotNull(organisation, "organisation");

            return new
            {
                organisation.Id,
                organisation.Name,
                organisation.Description,
                organisation.Website,
                Avatar = _avatarFactory.Make(organisation)
            };
        }

        #endregion
    }
}