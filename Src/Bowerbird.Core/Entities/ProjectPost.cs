using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class ProjectPost : EntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected ProjectPost() : base() { }

        public ProjectPost(
            Project project,
            Post post,
            User editor
            )
            : base()
        {
            Contract.RequireNotNull(project, "project");
            Contract.RequireNotNull(post, "post");
            Contract.RequireNotNull(editor, "editor");

            Project = project;
            Post = post;
            CreatedByUser = editor;
            CreatedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Project Project { get; set; }

        [Required]
        public virtual Post Post { get; set; }

        [Required]
        public virtual User CreatedByUser { get; set; }

        [Required]
        public virtual DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}