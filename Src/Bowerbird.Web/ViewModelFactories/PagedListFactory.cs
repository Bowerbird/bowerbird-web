using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.ViewModelFactories
{
    public class PagedListFactory : IPagedListFactory
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public PagedList<T> Make<T>()
        {
            return new PagedList<T>()
            {
                TotalResultCount = 0
            };
        }

        public PagedList<T> Make<T>(
            int page, 
            int pageSize, 
            int totalResultCount, 
            IEnumerable<T> pageObjects, 
            IDictionary<int, string> namedPages)
        {
            Check.RequireNotNull(pageObjects, "pageObjects");

            var allPageNumbers = MakeAllPageNumbers(page, pageSize, totalResultCount, namedPages);
            var pagedListItems = MakePagedListItems(page, pageSize, pageObjects, allPageNumbers);

            return new PagedList<T>()
            {
                PageSize = pageSize,
                TotalResultCount = totalResultCount,
                AllPageNumbers = allPageNumbers,
                PagedListItems = pagedListItems
            };
        }

        private List<PageNumber> MakeAllPageNumbers(
            int page, 
            int pageSize, 
            int totalResultCount, 
            IDictionary<int, string> namedPages)
        {
            List<PageNumber> allPageNumbers = new List<PageNumber>();

            // Create page numbers for all pages including those which are not loaded from persistence
            if (totalResultCount > 0)
            {
                int totalPageCount = (totalResultCount / pageSize) + (totalResultCount % pageSize > 0 ? 1 : 0);

                for (int pageNumber = 1; pageNumber <= totalPageCount; pageNumber++)
                {
                    int pageStartNumber = 1 + ((pageNumber * pageSize) - pageSize);
                    int pageEndNumber;
                    if (pageNumber == totalPageCount)
                    {
                        // Last page - get the page end number right
                        pageEndNumber = Convert.ToInt32(totalResultCount);
                    }
                    else
                    {
                        pageEndNumber = pageNumber * pageSize;
                    }

                    string name = namedPages != null ? namedPages[pageNumber] : string.Empty;

                    allPageNumbers.Add(MakePageNumber(pageNumber, pageStartNumber, pageEndNumber, page, name));
                }
            }

            return allPageNumbers;
        }

        private PageNumber MakePageNumber(
            int pageNumber, 
            int pageStartNumber, 
            int pageEndNumber, 
            int requestedPageStartNumber, 
            string name)
        {
            return new PageNumber(pageNumber,
                                    pageStartNumber,
                                    pageEndNumber,
                                    requestedPageStartNumber == pageStartNumber,
                                    name);
        }

        private IList<PagedListItem<T>> MakePagedListItems<T>(
            int page, 
            int pageSize, 
            IEnumerable<T> pageObjects, 
            IList<PageNumber> allPageNumbers)
        {
            List<PagedListItem<T>> pagedListItems = new List<PagedListItem<T>>();

            int pageObjectPosition = page;
            for (int pageNumber = 1; pageNumber <= pageSize; pageNumber++)
            {
                int pageObjectStartIndex = (pageNumber * pageSize) - pageSize;

                IList<T> pageObjectsToAdd = pageObjects
                    .Skip(pageObjectStartIndex)
                    .Take(pageSize)
                    .ToList();

                // TODO: Deal with pageStartNumber outside of result set!
                foreach (T pageObject in pageObjectsToAdd)
                {
                    pagedListItems.Add(MakePagedListItem(allPageNumbers.Single(p => p.Number == pageNumber), pageObjectPosition++, pageObject));
                }
            }

            return pagedListItems;
        }

        private PagedListItem<T> MakePagedListItem<T>(
            PageNumber pageNumber, 
            int pageObjectPosition, 
            T pageObject)
        {
            return new PagedListItem<T>(
                pageNumber,
                pageObjectPosition,
                pageObject);
        }

        #endregion

    }

}