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

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public class DenormalisedGroupReference
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; private set; }

        public string GroupType { get; private set; }

        public string CreatedBy { get; set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedGroupReference(Group group)
        {
            Check.RequireNotNull(group, "group");

            return new DenormalisedGroupReference
            {
                Id = group.Id,
                GroupType = group.GroupType,
                CreatedBy = group.User != null ? group.User.Id : null // Need this check only bevcause of AppRoot not having a User
            };
        }

        #endregion
    }
}