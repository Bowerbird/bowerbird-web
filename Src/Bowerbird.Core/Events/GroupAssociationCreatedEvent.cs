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
    public class GroupAssociationCreatedEvent : DomainEventBase
    {
        #region Members

        #endregion

        #region Constructors

        public GroupAssociationCreatedEvent(
            Group parentGroup,
            Group childGroup,
            User createdByUser, 
            DomainModel sender)
            : base(
            createdByUser,
            sender)
        {
            Check.RequireNotNull(parentGroup, "parentGroup");
            Check.RequireNotNull(childGroup, "childGroup");

            ParentGroup = parentGroup;
            ChildGroup = childGroup;
        }

        #endregion

        #region Properties

        public Group ParentGroup { get; private set; }

        public Group ChildGroup { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}