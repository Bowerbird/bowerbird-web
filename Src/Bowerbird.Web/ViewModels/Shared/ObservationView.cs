/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class ObservationView : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("observedOn")]
        public DateTime ObservedOn { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("isIdentificationRequired")]
        public bool IsIdentificationRequired { get; set; }

        [JsonProperty("observationCategory")]
        public string ObservationCategory { get; set; }

        [JsonProperty("observationMedia")]
        public IEnumerable<ObservationMediaItem> ObservationMedia { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}