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
using System.Collections.Generic;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class StreamItem
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("createdDateTimeDescription")]
        public string CreatedDateTimeDescription { get; set; }

        [JsonProperty("user")]
        public UserProfile User { get; set; }

        [JsonProperty("item")]
        public object Item { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("groups")]
        public IEnumerable<string> Groups { get; set; }
        
        #endregion

        #region Methods

        #endregion      
    }
}