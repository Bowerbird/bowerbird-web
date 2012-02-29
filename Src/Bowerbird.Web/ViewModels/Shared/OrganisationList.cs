/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Paging;

namespace Bowerbird.Web.ViewModels.Shared
{
    public class OrganisationList
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public int Page { get; set; }

        public int PageSize { get; set; }

        public PagedList<OrganisationView> Organisations { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}