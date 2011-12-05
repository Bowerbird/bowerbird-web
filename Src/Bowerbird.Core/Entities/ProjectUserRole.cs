using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class ProjectUserRole : EntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected ProjectUserRole()
            : base()
        {
        }

        public ProjectUserRole(
            ProjectUser projectUser,
            Role role,
            User createdByUser
            )
        {
            Contract.RequireNotNull(projectUser, "projectUser");
            Contract.RequireNotNull(role, "role");
            Contract.RequireNotNull(createdByUser, "createdByUser");

            ProjectUser = projectUser;
            Role = role;
            CreatedByUser = createdByUser;
            CreatedDateTime = DateTime.Now;
            ModifiedByUser = createdByUser;
            ModifiedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        [Required]
        public virtual DateTime CreatedDateTime { get; set; }

        [Required]
        public virtual DateTime ModifiedDateTime { get; set; }

        [Required]
        public virtual User CreatedByUser { get; set; }

        [Required]
        public virtual User ModifiedByUser { get; set; }

        [Required]
        public virtual ProjectUser ProjectUser { get; set; }

        [Required]
        public virtual Role Role { get; set; }

        public virtual bool IsDisabled { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}