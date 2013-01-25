/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Linq;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Member
    {
        #region Members

        #endregion

        #region Constructors

        protected Member()
            : base()
        {
            InitMembers();
        }

        public Member(
            User createdByUser,
            DateTime created,
            Group group,
            IEnumerable<Role> roles)
            : base()
        {
            Check.RequireNotNull(group, "group");
            Check.Require(roles != null && roles.ToList().Count > 0, "role collection must be not null and must contain at least one role items");

            InitMembers();

            Group = group;
            Roles = roles;
            Created = created;
        }

        #endregion

        #region Properties

        public DenormalisedGroupReference Group { get; private set; }

        public IEnumerable<Role> Roles { get; private set; }

        public DateTime Created { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Roles = new List<Role>();
        }

        public Member UpdateRoles(IEnumerable<Role> roles)
        {
            Check.RequireNotNull(roles, "roles");

            var existingRoles = ((List<Role>) Roles);

            existingRoles.Clear();
            existingRoles.AddRange(roles);

            return this;
        }

        #endregion
    }
}