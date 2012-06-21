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
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Member : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected Member()
            : base()
        {
            InitMembers();

            EnableEvents();
        }

        public Member(
            User createdByUser,
            User user,
            Group group,
            IEnumerable<Role> roles)
            : base()
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(group, "group");
            Check.Require(roles != null && roles.ToList().Count > 0, "role collection must be not null and contain role items");

            InitMembers();

            User = user;
            Group = group;
            Roles = roles;

            EnableEvents();
            FireEvent(new DomainModelCreatedEvent<Member>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DenormalisedGroupReference Group { get; private set; }

        public IEnumerable<Role> Roles { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Roles = new List<Role>();
        }

        public Member AddRole(Role role)
        {
            Check.RequireNotNull(role, "role");

            SetRole(role);

            return this;
        }

        /// <summary>
        /// Used by private User member to insert already denormalised roles
        /// </summary>
        internal Member AddRoles(IEnumerable<Role> roles)
        {
            Check.RequireNotNull(roles, "roles");

            foreach (var role in roles)
            {
                SetRole(role);
            }

            return this;
        }

        public Member RemoveRole(string roleId)
        {
            ((List<Role>)Roles).RemoveAll(x => x.Id == roleId);

            return this;
        }

        private void SetRole(Role role)
        {
            if (((List<Role>)Roles).All(x => x.Id != role.Id))
            {
                ((List<Role>)Roles).Add(role);
            }
        }

        #endregion
    }
}