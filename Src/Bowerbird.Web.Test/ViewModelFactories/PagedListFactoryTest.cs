using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.ViewModels;
using NUnit.Framework;

namespace Bowerbird.Web.Test.ViewModelFactories
{
    [TestFixture]
    public class PagedListFactoryTest
    {
            
        #region Infrastructure

        [SetUp]
        public void TestInitialize()
        {

        }

        [TearDown]
        public void TestCleanup()
        {

        }

        #endregion

        #region Helpers

        private List<object> CreatePageItems()
        {
            var pageItems = new List<object>();

            for (var i = 1; i <= 100; i++) 
                pageItems.Add(new { Id = i, Content = Guid.NewGuid() });

            return pageItems;
        }

        #endregion

        #region Constructor tests


        #endregion

        #region Property tests


        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)] 
        public void PagedListFactory_Make_With_Type_Object_Returns_PagedList_Of_Type_Object()
        {
            var pagedListFactory = new PagedListFactory();

            var pagedList = pagedListFactory.Make<object>();

            Assert.IsTrue(pagedList.GetType().Equals(typeof(PagedList<object>)));
        }

        [Test, Category(TestCategory.Unit)]
        public void PagedListFactory_Make_Returns_PagedList_Having_TotalResultCount_Of_Zero()
        {
            var pagedListFactory = new PagedListFactory();

            var pagedList = pagedListFactory.Make<object>();

            Assert.AreEqual(pagedList.TotalResultCount, 0);
        }

        [Test, Category(TestCategory.Unit)]
        public void PagedListFactory_Make_Passing_Null_PageObjects_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(
                            () => new PagedListFactory()
                                .Make<object>(
                                    FakeValues.Page,
                                    FakeValues.PageSize,
                                    100,
                                    null,
                                    null
                                )
                            ));
        }

        [Test, Category(TestCategory.Unit)]
        public void PagedListFactory_Make_Passing_PageObjects_Returns_PagedList_With_Specified_Size_And_Count()
        {
            List<object> pageItems = CreatePageItems();

            var pagedList = new PagedListFactory()
                .Make<object>(
                    FakeValues.Page,
                    FakeValues.PageSize,
                    pageItems.Count,
                    pageItems.Take(10),
                    null
                );

            Assert.AreEqual(pagedList.TotalResultCount, pageItems.Count);
            Assert.AreEqual(pagedList.PageSize, FakeValues.PageSize);
            Assert.AreEqual(pagedList.PagedListItems.GetEnumeratorCount(), FakeValues.PageSize);
        }

        #endregion					
				
    }
}