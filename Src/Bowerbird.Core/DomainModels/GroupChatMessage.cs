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
    public class GroupChatMessage : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        protected GroupChatMessage()
        {
        }

        public GroupChatMessage(
            User user,
            Group group,
            User targetUser,
            DateTime timestamp,
            string message
            )
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(group, "group");

            User = user;
            GroupId = group.Id;

            if(targetUser != null)TargetUser = targetUser;
            
            Timestamp = timestamp;
            Message = message;
        }

        #endregion

        #region Properties

        public string Message { get; private set; }

        public string GroupId { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public DenormalisedUserReference TargetUser { get; private set; }
        
        public DateTime Timestamp { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}