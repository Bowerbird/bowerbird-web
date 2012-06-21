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
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class GroupAssociation : ValueObject
    {
        #region Members

        #endregion

        #region Constructors

        protected GroupAssociation()
            : base()
        {
        }

        public GroupAssociation(
            Group parentGroup,
            Group childGroup,
            User createdByUser,
            DateTime createdDateTime)
            : base()
        {
            Check.RequireNotNull(parentGroup, "parentGroup");
            Check.RequireNotNull(childGroup, "childGroup");
            Check.RequireNotNull(createdByUser, "createdByUser");

            ParentGroup = parentGroup;
            ChildGroup = childGroup;
            CreatedByUserId = createdByUser.Id;
            CreatedDateTime = createdDateTime;
        }

        #endregion

        #region Properties

        public DenormalisedGroupReference ParentGroup { get; private set; }

        public DenormalisedGroupReference ChildGroup { get; private set; }

        public string CreatedByUserId { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        #endregion

        #region Methods

        #endregion      
    }
}