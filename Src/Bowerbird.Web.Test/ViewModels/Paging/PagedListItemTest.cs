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
    public class PagedListItemTest
    {
        #region Infrastructure

        [SetUp] 
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Helpers

        private PageNumber TestPageNumber()
        {
            return new PageNumber(
                FakeValues.Number,
                FakeValues.Number,
                FakeValues.Number,
                FakeValues.IsTrue,
                FakeValues.Name);
        }

        private PagedListItem<object> TestPagedListItem()
        {
            return new PagedListItem<object>(
                TestPageNumber(),
                FakeValues.Number,
                new object()
                );
        }

        #endregion

        #region Constructor tests

        [Test, Category(TestCategories.Unit)] 
        public void PagedListItem_Constructor_Sets_Properties()
        {
            var pageObject = new object();
            var pageNumber = TestPageNumber();

            var pagedListItem = new PagedListItem<object>(
                pageNumber,
                FakeValues.Number,
                pageObject
                );

            Assert.AreEqual(pagedListItem.PageNumber, pageNumber);
            Assert.AreEqual(pagedListItem.Position, FakeValues.Number);
            Assert.AreEqual(pagedListItem.PageObject, pageObject);
        }

        #endregion

        #region Property tests

        [Test, Category(TestCategories.Unit)] 
        public void PagedListItem_PageNumber_Is_A_PageNumber()
        {
            Assert.IsInstanceOf<PageNumber>(TestPagedListItem().PageNumber);
        }

        [Test, Category(TestCategories.Unit)]
        public void PagedListItem_Position_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(TestPagedListItem().Position);
        }

        [Test, Category(TestCategories.Unit)]
        public void PagedListItem_When_Typed_As_An_Object_Has_PageObject_Of_Type_Object()
        {
            Assert.IsInstanceOf<object>(TestPagedListItem().PageObject);
        }

        #endregion

        #region Method tests

        #endregion
    }
}