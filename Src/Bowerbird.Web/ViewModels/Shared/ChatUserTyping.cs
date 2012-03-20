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
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class ChatUserTyping
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("id")]
        public string ChatId { get; set; }

        [JsonProperty("user")]
        public UserProfile User { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("typing")]
        public bool Typing { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}
