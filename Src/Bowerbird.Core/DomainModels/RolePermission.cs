using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class RolePermission : EntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected RolePermission()
            : base()
        {
        }

        public RolePermission(
            Role role,
            Permission permission
            )
        {
            Contract.RequireNotNull(role, "role");
            Contract.RequireNotNull(permission, "permission");

            Role = role;
            Permission = permission;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Role Role { get; set; }

        [Required]
        public virtual Permission Permission { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}