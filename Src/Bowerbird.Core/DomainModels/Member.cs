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
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class Member : DomainModel
    {
        #region Members

        [JsonIgnore]
        private List<DenormalisedRoleReference> _roles;

        #endregion

        #region Constructors

        protected Member()
            : base()
        {
            InitMembers();
        }

        public Member(
            User createdByUser,
            User user,
            Group group,
            IEnumerable<Role> roles)
            : this()
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(group, "group");
            Check.Require(roles != null && roles.ToList().Count > 0, "role collection must be not null and contain role items");

            User = user;
            Group = group;
            Roles = roles.Select(x => (DenormalisedRoleReference)x).ToList();

            EventProcessor.Raise(new DomainModelCreatedEvent<Member>(this, createdByUser.Id));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DenormalisedNamedDomainModelReference<Group> Group { get; private set; }

        public IEnumerable<DenormalisedRoleReference> Roles 
        {
            get { return _roles; }
            private set { _roles = new List<DenormalisedRoleReference>(value); }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            _roles = new List<DenormalisedRoleReference>();
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
        internal Member AddRoles(IEnumerable<DenormalisedRoleReference> roles)
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
            _roles.RemoveAll(x => x.Id == roleId);

            return this;
        }

        private void SetRole(DenormalisedRoleReference role)
        {
            if (_roles.All(x => x.Id != role.Id))
            {
                _roles.Add(role);
            }
        }

        #endregion
    }
}