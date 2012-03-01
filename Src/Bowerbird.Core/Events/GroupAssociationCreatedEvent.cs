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
    public class GroupAssociationCreatedEvent : IDomainEvent
    {
        #region Members

        #endregion

        #region Constructors

        public GroupAssociationCreatedEvent(
            Group parentGroup,
            Group childGroup,
            User createdByUser,
            string eventMessage)
        {
            Check.RequireNotNull(parentGroup, "parentGroup");
            Check.RequireNotNull(childGroup, "childGroup");
            Check.RequireNotNull(createdByUser, "createdByUser");

            ParentGroup = parentGroup;
            ChildGroup = childGroup;
            CreatedByUser = createdByUser;
            EventMessage = eventMessage;
        }

        #endregion

        #region Properties

        public Group ParentGroup { get; private set; }

        public Group ChildGroup { get; private set; }

        public User CreatedByUser { get; private set; }

        public string EventMessage { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}