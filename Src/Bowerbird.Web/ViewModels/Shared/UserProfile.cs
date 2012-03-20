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
    public class UserProfile
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastLoggedIn")]
        public DateTime LastLoggedIn { get; set; }

        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}