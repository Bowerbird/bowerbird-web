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
    public class ChatMessage : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        protected ChatMessage()
        {
        }

        public ChatMessage(
            User user,
            DateTime timestamp,
            string message)
        {
            Check.RequireNotNull(user, "user");

            User = user;
            Timestamp = timestamp;
            Message = message;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime Timestamp { get; private set; }

        public string Message { get; private set; }
        
        #endregion

        #region Methods

        #endregion
    }
}