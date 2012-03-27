/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.Members;

namespace Bowerbird.Core.DomainModels.DenormalisedReferences
{
    public class DenormalisedGroupMemberReference : ValueObject
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; private set; }

        public IEnumerable<string> Roles { get; private set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedGroupMemberReference(GroupMember groupMember)
        {
            Check.RequireNotNull(groupMember, "groupMember");

            return new DenormalisedGroupMemberReference
            {
                Id = groupMember.Id,
                Roles = groupMember.Roles.Select(x => x.Id)
            };
        }

        #endregion
    }
}