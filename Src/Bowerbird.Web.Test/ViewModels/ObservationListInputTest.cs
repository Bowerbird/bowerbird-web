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

    [TestFixture]
    public class ObservationListInputTest
    {
        #region Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test]
        public void ObservationListInput_Username_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationListInput() { Username = FakeValues.UserName }.Username);
        }

        [Test]
        public void ObservationListInput_Pagesize_Is_An_Int()
        {
            Assert.IsInstanceOf<string>(new ObservationListInput() { PageSize = FakeValues.PageSize }.PageSize);
        }

        [Test]
        public void ObservationListInput_Page_Is_An_Int()
        {
            Assert.IsInstanceOf<string>(new ObservationListInput() { Page = FakeValues.Page }.Page);
        }

        #endregion

        #region Method tests

        #endregion

    }
}