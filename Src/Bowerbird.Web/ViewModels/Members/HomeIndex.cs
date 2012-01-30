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
using Bowerbird.Core.Paging;
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

        public PagedList<StreamItemViewModel> StreamItems { get; set; }

        public UserProfile UserProfile { get; set; }

        public List<MenuItem> ProjectMenu { get; set; }

        public List<MenuItem> TeamMenu { get; set; }

        public List<MenuItem> WatchlistMenu { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}