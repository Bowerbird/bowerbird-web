using System;
using System.Collections.Generic;
using System.Linq;

namespace Bowerbird.Web.ViewModels
{
    public class PagedList<T> : IEnumerable<PagedListItem<T>>
    {

        #region Fields

        #endregion

        #region Constructors

        public PagedList()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public IEnumerable<PagedListItem<T>> PagedListItems { get; set; }

        public int PageSize { get; set; }

        public int TotalResultCount { get; set; }

        public List<PageNumber> AllPageNumbers { get; set; }

        public PageNumber PreviousPageNumber
        {
            get
            {
                if (SelectedPageNumber != null && SelectedPageNumber.PageStartNumber > 1)
                {
                    return AllPageNumbers.SingleOrDefault(p => p.Number == SelectedPageNumber.Number - 1);
                }

                return null;
            }
        }

        public PageNumber NextPageNumber
        {
            get
            {
                if (SelectedPageNumber != null && SelectedPageNumber.PageEndNumber < TotalResultCount)
                {
                    return AllPageNumbers.SingleOrDefault(p => p.Number == SelectedPageNumber.Number + 1);
                }

                return null;
            }
        }

        public PageNumber SelectedPageNumber
        {
            get
            {
                return AllPageNumbers.SingleOrDefault(p => p.Selected);
            }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            PageSize = 10;
            TotalResultCount = 0;

            PagedListItems = new List<PagedListItem<T>>();
            AllPageNumbers = new List<PageNumber>();
        }

        public PagedList<TProjection> ConvertInto<TProjection>(Func<T, TProjection> pagedObjectConverter)
        {
            List<PagedListItem<TProjection>> newPagedListItems = new List<PagedListItem<TProjection>>();

            foreach (PagedListItem<T> pagedListItem in PagedListItems)
            {
                TProjection newPageObject = pagedObjectConverter(pagedListItem.PageObject);
                if (newPageObject == null)
                {
                    throw new NullReferenceException("The conversion function returned a null reference. Each paged element must be projected into a new non-null instance.");
                }
                newPagedListItems.Add(new PagedListItem<TProjection>(pagedListItem.PageNumber, pagedListItem.Position, newPageObject));
            }

            return new PagedList<TProjection>()
                       {
                           AllPageNumbers = this.AllPageNumbers,
                           PageSize = this.PageSize,
                           TotalResultCount = this.TotalResultCount,
                           PagedListItems = newPagedListItems
                       };
        }

        /// <summary>
        /// Gets a list of page numbers on either side of the selected page number. If the window hangs over either end of 
        /// the page number list, the window is "slid" to return the requested pageNumberWindowSize. Returns all page 
        /// numbers if no selected page number.
        /// </summary>
        /// <param name="pageNumberWindowSize">pageNumberWindowSize must be an odd number greater than one</param>
        public IList<PageNumber> GetPageNumberWindow(int pageNumberWindowSize)
        {
            //Check.Require(pageNumberWindowSize > 0, "pageNumberWindowSize may not be less than one");
            //Check.Require((pageNumberWindowSize & 1) != 0, "pageNumberWindowSize may not be an even number");

            if (SelectedPageNumber != null)
            {
                int windowPageStart = SelectedPageNumber.Number - ((pageNumberWindowSize - 1) / 2);
                int windowPageEnd = SelectedPageNumber.Number + ((pageNumberWindowSize - 1) / 2);

                if (windowPageStart < 1)
                {
                    while (windowPageStart < 1)
                    {
                        windowPageStart++;
                        windowPageEnd++;
                    }
                }

                if (windowPageEnd > AllPageNumbers.Last().Number)
                {
                    while (windowPageEnd > AllPageNumbers.Last().Number)
                    {
                        windowPageStart--;
                        windowPageEnd--;
                    }
                }

                return (from pageNumber in AllPageNumbers
                        where pageNumber.Number >= windowPageStart && pageNumber.Number <= windowPageEnd
                        select pageNumber)
                       .ToList();
            }

            return AllPageNumbers.ToList();
        }

        public IEnumerable<T> GetPagedObjects()
        {
            return PagedListItems.Select(x => x.PageObject);
        }

        public IEnumerator<PagedListItem<T>> GetEnumerator()
        {
            return this.PagedListItems.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
