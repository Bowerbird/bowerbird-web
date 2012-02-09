/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class GroupAssociation : ValueObject
    {

        #region Members

        #endregion

        #region Constructors

        protected GroupAssociation()
            :base()
        {
        }

        public GroupAssociation(
            Group group,
            User createdByUser,
            DateTime createdDateTime)
            : base()
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(createdByUser, "createdByUser");

            GroupId = group.Id;
            CreatedByUserId = createdByUser.Id;
            CreatedDateTime = createdDateTime;

            EventProcessor.Raise(new DomainModelCreatedEvent<GroupAssociation>(this, createdByUser));
        }

        #endregion

        #region Properties

        public string GroupId { get; private set; }

        public string CreatedByUserId { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
