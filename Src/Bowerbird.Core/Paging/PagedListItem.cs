namespace Bowerbird.Core.Paging
{
    public class PagedListItem<T>
    {

        #region Fields

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