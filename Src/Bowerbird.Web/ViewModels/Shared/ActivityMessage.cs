/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModels.Shared
{
    public class ActivityMessage
    {
        public string Sender { get; set; }//observation, group, user

        public string Action { get; set; }//created,updated,deleted,joined,loggedin,commented

        public string GroupId { get; set; }

        public string ContributionId { get; set; }

        public string WatchlistId { get; set; }

        public UserProfile User { get; set; }

        public Avatar Avatar { get; set; }

        public string Message { get; set; }//ken walker created the 'bees in carlton' project
    }
}