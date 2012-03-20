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
    public class PrivateChatMessage : ValueObject
    {
        #region Fields

        #endregion

        #region Constructors

        protected PrivateChatMessage()
        {
        }

        public PrivateChatMessage(
            User user,
            string chatId,
            User targetUser,
            string message,
            DateTime timestamp
            )
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNullOrWhitespace(message, "message");

            User = user;
            TargetUser = targetUser;
            Message = message;
            Timestamp = timestamp;
            ChatId = chatId;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; set; }

        public DenormalisedUserReference TargetUser { get; set; }

        public string ChatId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}