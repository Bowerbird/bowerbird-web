using System;
using System.ComponentModel.DataAnnotations;
using Bowerbird.Common.Utils;

namespace Bowerbird.Core.Entities
{
    public class TeamObservation : EntityBase
    {

        #region Fields

        #endregion

        #region Constructors

        /// <summary>
        /// Nhibernate required constructor
        /// </summary>
        protected TeamObservation( ) : base ( ) { }

        public TeamObservation(
            Team group,
            Observation observation,
            User editor
            )
            :base()
        {
            Contract.RequireNotNull(group, "group");
            Contract.RequireNotNull(observation, "observation");
            Contract.RequireNotNull(editor, "createdByUser");

            Team = group;
            Observation = observation;
            CreatedByUser = editor;
            CreatedDateTime = DateTime.Now;
        }

        #endregion

        #region Properties

        [Required]
        public virtual Observation Observation { get; set; }

        [Required]
        public virtual Team Team { get; set; }

        [Required]
        public virtual User CreatedByUser { get; set; }

        [Required]
        public virtual DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}