using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class TeamProject : EntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected TeamProject() : base() { }

        public TeamProject(
            Team group,
            Project project,
            User editor
            )
            : base()
        {
            Contract.RequireNotNull(group, "group");
            Contract.RequireNotNull(project, "project");
            Contract.RequireNotNull(editor, "editor");

            Team = group;
            Project = project;
            CreatedByUser = editor;
            CreatedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Team Team { get; set; }

        [Required]
        public virtual Project Project { get; set; }

        [Required]
        public virtual User CreatedByUser { get; set; }

        [Required]
        public virtual DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}