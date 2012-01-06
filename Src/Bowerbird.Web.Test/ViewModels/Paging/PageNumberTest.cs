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
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.MediaResources;

    #endregion

    [TestFixture] 
    public class PageNumberTest
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

        #endregion

        #region Constructor tests

        [Test, Category(TestCategory.Unit)] 
        public void PageNumber_Constructor_Sets_Properties()
        {
            var pageNumber = new PageNumber(
                FakeValues.Number,
                FakeValues.Number,
                FakeValues.Number,
                FakeValues.IsTrue,
                FakeValues.Name);

            Assert.AreEqual(pageNumber.Name, FakeValues.Name);
            Assert.AreEqual(pageNumber.Number, FakeValues.Number);
            Assert.AreEqual(pageNumber.PageEndNumber, FakeValues.Number);
            Assert.AreEqual(pageNumber.PageStartNumber, FakeValues.Number);
            Assert.AreEqual(pageNumber.Selected, FakeValues.IsTrue);
        }

        #endregion

        #region Property tests

        [Test, Category(TestCategory.Unit)] 
        public void PageNumber_Name_Is_A_String()
        {
            Assert.IsInstanceOf<string>(TestPageNumber().Name);
        }

        [Test, Category(TestCategory.Unit)] 
        public void PageNumber_Number_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(TestPageNumber().Number);
        }

        [Test, Category(TestCategory.Unit)] 
        public void PageNumber_PageEndNumber_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(TestPageNumber().PageEndNumber);
        }

        [Test, Category(TestCategory.Unit)] 
        public void PageNumber_PageStartNumber_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(TestPageNumber().PageStartNumber);
        }

        [Test, Category(TestCategory.Unit)] 
        public void PageNumber_Selected_Is_A_Bool()
        {
            Assert.IsInstanceOf<bool>(TestPageNumber().Selected);
        }

        #endregion

        #region Method tests

        #endregion
    }
}