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
using Bowerbird.Core.Indexes;

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

        /// <summary>
        /// Return the Organisation properties and avatar
        /// passing the Organisation Domain Model
        /// </summary>
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

        /// <summary>
        /// Return the Organisation properties and avatar with
        /// Team count and Member count
        /// passing the index All_Groups.Result
        /// </summary>
        public object Make(All_Groups.Result organisation)
        {
            Check.RequireNotNull(organisation, "organisation");

            return new
            {
                organisation.Id,
                organisation.Organisation.Name,
                organisation.Organisation.Description,
                organisation.Organisation.Website,
                Avatar = _avatarFactory.Make(organisation.Organisation),
                organisation.GroupMemberCount,
                Teams = 0,
                Projects = 0
            };
        }

        #endregion
    }
}