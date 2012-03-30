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
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class ObservationGroup : ValueObject
    {
        #region Members

        #endregion

        #region Constructors

        protected ObservationGroup()
        {
        }

        public ObservationGroup(
            Group group,
            User createdByUser,
            DateTime createdDateTime)
            : this()
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(createdByUser, "createdByUser");

            GroupType = group.GetType().Name.ToLower();
            GroupId = group.Id;
            User = createdByUser;
            CreatedDateTime = createdDateTime;
        }

        #endregion

        #region Properties

        public string GroupId { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public string GroupType { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}