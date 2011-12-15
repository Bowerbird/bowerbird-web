using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities.DenormalisedReferences;
using System;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.Entities
{
    public abstract class Member : Entity
    {

        #region Members

        #endregion

        #region Constructors

        protected Member()
            : base()
        {
            InitMembers();
        }

        protected Member(
            User user,
            IEnumerable<Role> roles)
            : this()
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(roles, "roles");

            SetDetails(
                user);

            Roles = roles.Select(x => (DenormalisedNamedEntityReference<Role>)x).ToList();
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public List<DenormalisedNamedEntityReference<Role>> Roles { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Roles = new List<DenormalisedNamedEntityReference<Role>>();
        }

        protected void SetDetails(User user)
        {
            User = user;
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
        internal Member AddRoles(IEnumerable<DenormalisedNamedEntityReference<Role>> roles)
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
            Roles.RemoveAll(x => x.Id == roleId);

            return this;
        }

        private void SetRole(DenormalisedNamedEntityReference<Role> role)
        {
            if (Roles.All(x => x.Id != role.Id))
            {
                Roles.Add(role);
            }
        }

        #endregion

    }
}