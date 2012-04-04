using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class ObservationMediaItem
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("mediaResourceId")]
        public string MediaResourceId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("licence")]
        public string Licence { get; set; }

        [JsonProperty("originalImageUri")]
        public string OriginalImageUri { get; set; }

        [JsonProperty("largeImageUri")]
        public string LargeImageUri { get; set; }

        [JsonProperty("mediumImageUri")]
        public string MediumImageUri { get; set; }

        [JsonProperty("smallImageUri")]
        public string SmallImageUri { get; set; }

        [JsonProperty("thumbnailImageUri")]
        public string ThumbnailImageUri { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
