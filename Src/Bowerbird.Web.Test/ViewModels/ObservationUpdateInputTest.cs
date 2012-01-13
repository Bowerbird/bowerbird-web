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
    public class ObservationUpdateInputTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize()
        {
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_ObservationId_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationUpdateInput() { ObservationId = FakeValues.KeyString }.ObservationId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_Address_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationUpdateInput() { Address = FakeValues.Address }.Address);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_Latitude_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationUpdateInput() { Latitude = FakeValues.Latitude }.Latitude);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_Longitude_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationUpdateInput() { Longitude = FakeValues.Longitude }.Longitude);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_Title_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationUpdateInput() { Title = FakeValues.Title }.Title);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_ObservationCategory_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationUpdateInput() { ObservationCategory = FakeValues.Category }.ObservationCategory);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_ObservedOn_Is_A_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(new ObservationUpdateInput() { ObservedOn = FakeValues.CreatedDateTime }.ObservedOn);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_UserId_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationUpdateInput() { UserId = FakeValues.UserId }.UserId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateInput_MediaResources_Is_A_String_List()
        {
            Assert.IsInstanceOf<List<string>>(new ObservationUpdateInput() { MediaResources = FakeValues.StringList }.MediaResources);
        }

        #endregion

        #region Method tests

        #endregion
    }
}