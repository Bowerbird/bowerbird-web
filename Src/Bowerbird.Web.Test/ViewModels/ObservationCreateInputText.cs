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
    public class ObservationCreateInputTest
    {
        #region Test Infrastructure

        [SetUp] 
        public void TestInitialize() { }

        [TearDown] 
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_Address_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationCreateInput() { Address = FakeValues.Address }.Address );
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_Latitude_Is_A_String() 
        {
            Assert.IsInstanceOf<string>(new ObservationCreateInput() { Latitude = FakeValues.Latitude }.Latitude); 
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_Longitude_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationCreateInput() { Longitude = FakeValues.Longitude }.Longitude);
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_Title_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationCreateInput() { Title = FakeValues.Title }.Title);
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_ObservationCategory_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationCreateInput() { ObservationCategory = FakeValues.Category }.ObservationCategory );
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_ObservedOn_Is_A_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(new ObservationCreateInput() { ObservedOn = FakeValues.CreatedDateTime }.ObservedOn);
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_Username_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ObservationCreateInput() { Username = FakeValues.UserName }.Username );
        }

        [Test, Category(TestCategory.Unit)] 
        public void ObservationCreateInput_MediaResources_Is_A_String_List()
        {
            Assert.IsInstanceOf<List<string>>(new ObservationCreateInput() { MediaResources = FakeValues.StringList }.MediaResources );
        }

        #endregion

        #region Method tests

        #endregion
    }
}