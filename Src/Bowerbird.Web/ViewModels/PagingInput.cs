/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;

namespace Bowerbird.Web.ViewModels
{
    public class PagingInput
    {
        #region Fields

        #endregion

        #region Constructors

        public PagingInput()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public int Page { get; set; }
        
        public int PageSize { get; set; }

        public string SortField { get; set; }

        public string SortDirection { get; set; }

        #endregion

        #region Methods

        public void InitMembers()
        {
            Page = DefaultPaging.PageStart;
            PageSize = DefaultPaging.PageSize;
        }

        public int GetPage()
        {
            return Page > 0 ? Page : 1;
        }

        public int GetPageSize()
        {
            // Do not allow paging sizes of greater than 50 to avoid performance/ravendb issues
            return PageSize > 0 && PageSize <= 50 ? PageSize : 10;
        }

        public int GetSkipIndex()
        {
            return (GetPage() - 1) * GetPageSize();
        }

        #endregion
    }
}