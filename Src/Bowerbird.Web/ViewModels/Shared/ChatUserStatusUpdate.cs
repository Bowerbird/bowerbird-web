using System;
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class ChatUserStatusUpdate
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

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}
