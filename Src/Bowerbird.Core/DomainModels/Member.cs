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

namespace Bowerbird.Core.DomainModels
{
    public abstract class Member : DomainModel
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

            User = user;
            Roles = roles.Select(x => (DenormalisedNamedDomainModelReference<Role>)x).ToList();
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public List<DenormalisedNamedDomainModelReference<Role>> Roles { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Roles = new List<DenormalisedNamedDomainModelReference<Role>>();
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
        internal Member AddRoles(IEnumerable<DenormalisedNamedDomainModelReference<Role>> roles)
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

        private void SetRole(DenormalisedNamedDomainModelReference<Role> role)
        {
            if (Roles.All(x => x.Id != role.Id))
            {
                Roles.Add(role);
            }
        }

        #endregion
    }
}