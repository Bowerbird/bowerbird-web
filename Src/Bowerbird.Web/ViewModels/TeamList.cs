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
using Bowerbird.Core.Paging;

namespace Bowerbird.Web.ViewModels
{
    public class TeamList
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public Organisation Organisation { get; set; }

        public User User { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public PagedList<TeamView> Teams { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}