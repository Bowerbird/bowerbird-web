/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.DomainModels.Sessions
{
    public class PrivateChatSession : Session
    {
        #region Fields

        #endregion

        #region Constructors

        protected PrivateChatSession()
        {
        }

        public PrivateChatSession(
            User user,
            string clientId,
            string chatId
            )
            : base(user, clientId)
        {
            ChatId = chatId;
        }

        #endregion

        #region Properties

        public string ChatId { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}