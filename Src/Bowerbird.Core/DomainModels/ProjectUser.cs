using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class ProjectUser : UserCreatedEntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected ProjectUser() : base(){}

        public ProjectUser(
            Project project,
            User user,
            User editor,
            long projectUserKey
            )
            : base(editor)
        {
            Contract.RequireNotNull(project, "project");
            Contract.RequireNotNull(user, "user");
            Contract.RequireGreaterThanZero(projectUserKey, "projectUserKey");

            Project = project;
            User = user;
            ProjectUserKey = projectUserKey;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Project Project { get; set; }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual long ProjectUserKey { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}