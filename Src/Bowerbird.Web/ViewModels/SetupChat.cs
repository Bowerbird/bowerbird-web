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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels
{
    public class SetupChat
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        // groupId = chatId = id
        [JsonProperty("id")]
        public string ChatId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("users")]
        public List<UserProfile> Users { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}