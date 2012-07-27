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
    public class SightingGroup
    {
        #region Members

        #endregion

        #region Constructors

        protected SightingGroup()
            : base()
        {
        }

        public SightingGroup(
            Group group,
            User createdByUser,
            DateTime createdDateTime)
            : base()
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(createdByUser, "createdByUser");

            Group = group;
            User = createdByUser;
            CreatedDateTime = createdDateTime;
        }

        #endregion

        #region Properties

        public DenormalisedGroupReference Group { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}