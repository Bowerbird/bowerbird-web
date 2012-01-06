using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class ProjectObservation : EntityBase
    {

        #region Members

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected ProjectObservation() : base() { }

        public ProjectObservation(
            Project project
            ,Observation observation
            ,User editor
            )
            : base()
        {
            Contract.RequireNotNull(project, "project");
            Contract.RequireNotNull(observation, "observation");
            Contract.RequireNotNull(editor, "editor");

            Project = project;
            Observation = observation;
            CreatedByUser = editor;
            CreatedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Project Project { get; set; }

        [Required]
        public virtual Observation Observation { get; set; }
        
        [Required]
        public virtual User CreatedByUser { get; set; }

        [Required]
        public virtual DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}