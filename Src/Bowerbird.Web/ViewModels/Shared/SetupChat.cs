using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels.Shared
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
