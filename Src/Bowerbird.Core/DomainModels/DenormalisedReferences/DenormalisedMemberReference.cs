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
    public class DenormalisedMemberReference : ValueObject
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Type { get; private set; }

        public string Id { get; private set; }

        public IEnumerable<string> Roles { get; private set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedMemberReference(Member member)
        {
            Check.RequireNotNull(member, "member");

            return new DenormalisedMemberReference
            {
                Type = SetMemberType(member),
                Id = member.Id,
                Roles = member.Roles.Select(x => x.Id)
            };
        }

        private static string SetMemberType(Member member)
        {
            if (member is GroupMember) return "groupmember";

            return "globalmember";
        }

        #endregion
    }
}