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

namespace Bowerbird.Core.ViewModels
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

        #endregion

        #region Methods

        private void InitMembers()
        {
            Page = 1;
            PageSize = DefaultPaging.PageSize;
        }

        public int GetPage()
        {
            return Page > 0 ? Page : 1;
        }

        public int GetPageSize()
        {
            // Do not allow paging sizes of greater than 50 to avoid performance/ravendb issues
            return PageSize > 0 && PageSize <= DefaultPaging.PageMax ? PageSize : DefaultPaging.PageSize;
        }

        public int GetSkipIndex()
        {
            return (GetPage() - 1) * GetPageSize();
        }

        #endregion
    }
}