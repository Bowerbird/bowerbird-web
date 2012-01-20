/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class ProjectObservation : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected ProjectObservation() : base() { }

        public ProjectObservation(
            User createdByUser,
            DateTime timestamp,
            Project project,
            Observation observation
            )
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(project, "project");
            Check.RequireNotNull(observation, "observation");

            Project = project;
            Observation = observation;
            CreatedByUser = createdByUser;
            CreatedDateTime = timestamp;
        }

        #endregion

        #region Properties

        public DenormalisedNamedDomainModelReference<Project> Project { get; set; }

        public DenormalisedObservationReference Observation { get; set; }

        public DenormalisedUserReference CreatedByUser { get; set; }

        public DateTime CreatedDateTime { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}