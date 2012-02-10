/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Web.ViewModels.Shared;

namespace Bowerbird.Web.ViewModels.Members
{
    public class HomeIndex : IViewModel
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public UserProfile UserProfile { get; set; }

        public IEnumerable<MenuItem> ProjectMenu { get; set; }

        public IEnumerable<MenuItem> TeamMenu { get; set; }

        public IEnumerable<MenuItem> WatchlistMenu { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}