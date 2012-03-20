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
    public class ActivityMessage
    {
        [JsonProperty("sender")]
        public string Sender { get; set; }//observation, group, user

        [JsonProperty("action")]
        public string Action { get; set; }//created,updated,deleted,joined,loggedin,commented

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("contributionId")]
        public string ContributionId { get; set; }

        [JsonProperty("watchlistId")]
        public string WatchlistId { get; set; }

        [JsonProperty("user")]
        public UserProfile User { get; set; }

        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }//ken walker created the 'bees in carlton' project
    }
}