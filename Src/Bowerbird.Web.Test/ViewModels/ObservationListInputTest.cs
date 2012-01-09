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

namespace Bowerbird.Web.Test.ViewModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] public class ObservationListInputTest
    {
        #region Test Infrastructure

        [SetUp] public void TestInitialize() { }

        [TearDown] public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test, Category(TestCategory.Unit)] 
        public void ObservationListInput_Username_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationListInput() { UserId = FakeValues.UserId }.UserId);
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationListInput_Pagesize_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(new ObservationListInput() { PageSize = FakeValues.PageSize }.PageSize);
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationListInput_Page_Is_An_Int()
        {
            Assert.IsInstanceOf<int>(new ObservationListInput() { Page = FakeValues.Page }.Page);
        }

        #endregion

        #region Method tests

        #endregion
    }
}