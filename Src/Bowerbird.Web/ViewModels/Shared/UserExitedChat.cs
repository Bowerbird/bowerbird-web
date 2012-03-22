/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class UserExitedChat
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        // groupId = chatId = id
        [JsonProperty("id")]
        public string ChatId { get; set; }

        [JsonProperty("user")]
        public UserProfile User { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}