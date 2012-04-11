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
using Bowerbird.Core.DomainModels;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class Notification
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("occurredOn")]
        public DateTime OccurredOn { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("avatarUri")]
        public string AvatarUri { get; set; }

        [JsonProperty("summaryDescription")]
        public string SummaryDescription { get; set; }

        [JsonProperty("createdDateTimeDescription")]
        public string CreatedDateTimeDescription { get; set; }

        [JsonProperty("model")]
        public object Model { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}