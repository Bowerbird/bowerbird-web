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

namespace Bowerbird.Web.Test.ViewModels.Paging
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Moq;
    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.ViewModelFactories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Entities.MediaResources;

    #endregion

    [TestFixture]
    public class PagedListTest
    {
        #region Infrastructure

        [SetUp] 
        public void TestInitialize() { }

        [TearDown] 
        public void TestCleanup() { }

        #endregion

        #region Helpers

        private PageNumber FirstPage()
        {
            return new PageNumber(1, 1, 10, FakeValues.IsFalse, "First page");
        }

        private PageNumber SecondPage()
        {
            return new PageNumber(2, 11, 20, FakeValues.IsTrue, "Second page");
        }

        private PageNumber ThirdPage()
        {
            return new PageNumber(3, 21, 30, FakeValues.IsFalse, "Third page");
        }

        private List<PageNumber> Pages()
        {
            return new List<PageNumber>() { FirstPage(), SecondPage(), ThirdPage() };
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test, Category(TestCategories.Unit)] 
        public void PagedList_PagedListItems_Is_An_Enumerable_PagedListItem_Of_Specified_Type()
        {
            Assert.IsInstanceOf<IEnumerable<PagedListItem<object>>>(new PagedList<object>().PagedListItems);
        }

        [Test, Category(TestCategories.Unit)] 
        public void PagedList_PageSize_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(new PagedList<object>() { PageSize = FakeValues.Number }.PageSize);
        }

        [Test, Category(TestCategories.Unit)] 
        public void PagedList_TotalResultCount_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(new PagedList<object>() { TotalResultCount = FakeValues.Number }.TotalResultCount);
        }

        [Test, Category(TestCategories.Unit)] 
        public void PagedList_AllPageNumbers_Is_A_List_Of_PageNumbers()
        {
            Assert.IsInstanceOf<List<PageNumber>>(new PagedList<object>() {AllPageNumbers = Pages()}.AllPageNumbers);
        }

        [Test, Category(TestCategories.Unit)] 
        public void PagedList_PreviousPageNumber_Is_A_PageNumber()
        {
            Assert.IsInstanceOf<PageNumber>(new PagedList<object>() { AllPageNumbers = Pages(), TotalResultCount = 30 }.PreviousPageNumber);
        }

        [Test, Category(TestCategories.Unit)] 
        public void PagedList_NextPageNumber_Is_A_PageNumber()
        {
            Assert.IsInstanceOf<PageNumber>(new PagedList<object>() { AllPageNumbers = Pages(), TotalResultCount = 30}.NextPageNumber);
        }

        [Test, Category(TestCategories.Unit)] 
        public void PagedList_SelectedPageNumber_Is_A_PageNumber()
        {
            Assert.IsInstanceOf<PageNumber>(new PagedList<object>() { AllPageNumbers = Pages(), TotalResultCount = 30 }.SelectedPageNumber);
        }

        #endregion

        #region Method tests


        #endregion
    }
}