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

using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class SightingCreatedEvent : DomainModelCreatedEvent<Sighting>
    {
        #region Members

        #endregion

        #region Constructors

        public SightingCreatedEvent(
            Sighting sighting,
            User createdByUser, 
            object sender,
            IEnumerable<Project> projects)
            : base(
            sighting,
            createdByUser,
            sender)
        {
            Check.RequireNotNull(projects, "projects");

            Projects = projects;
        }

        #endregion

        #region Properties

        public IEnumerable<Project> Projects { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}