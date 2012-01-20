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

namespace Bowerbird.Core.Paging
{
    public class PagedListItem<T>
    {

        #region Members

        #endregion

        #region Constructors

        public PagedListItem(PageNumber pageNumber, int position, T pageObject)
        {
            PageNumber = pageNumber;
            Position = position;
            PageObject = pageObject;
        }

        #endregion

        #region Properties

        public PageNumber PageNumber { get; set; }

        public int Position { get; set; }

        public T PageObject { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}