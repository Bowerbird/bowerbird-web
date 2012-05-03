/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections;

namespace Bowerbird.Web.ViewModels
{
    public class ClientUserContext
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public object UserProfile { get; set; }

        public IEnumerable Projects { get; set; }

        public IEnumerable Teams { get; set; }

        public IEnumerable OnlineUsers { get; set; }

        public IEnumerable ProjectMenu { get; set; }

        public IEnumerable TeamMenu { get; set; }

        public IEnumerable WatchlistMenu { get; set; }

        public string ProjectsJson { get; set; }

        public string TeamsJson { get; set; }

        public string OnlineUsersJson { get; set; }

        public string UserProfileJson { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}