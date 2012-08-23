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

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class SightingGroupCreatedEvent : DomainModelCreatedEvent<SightingGroup>
    {
        #region Members

        #endregion

        #region Constructors

        public SightingGroupCreatedEvent(
            SightingGroup sightingGroup,
            User createdByUser, 
            object sender,
            Group group)
            : base(
            sightingGroup,
            createdByUser,
            sender)
        {
            Check.RequireNotNull(group, "group");

            Group = group;
        }

        #endregion

        #region Properties

        public Group Group { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}