/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Commands
{
    #region Namespaces

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ObservationUpdateCommandTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static ObservationUpdateCommand TestObservationUpdateCommand()
        {
            return new ObservationUpdateCommand()
            {
                Address = FakeValues.Address,
                Id = FakeValues.KeyString,
                IsIdentificationRequired = FakeValues.IsTrue,
                Latitude = FakeValues.Latitude.ToString(),
                Longitude = FakeValues.Longitude.ToString(),
                MediaResources = TestMediaResources(),
                ObservationCategory = FakeValues.Category,
                ObservedOn = FakeValues.CreatedDateTime,
                Title = FakeValues.Title,
                Username = FakeValues.UserName
            };
        }

        private static List<string> TestMediaResources()
        {
            return new List<string>() { "mediaresource/123", "mediaresource/124", "mediaresource/125" };
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_Id_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationUpdateCommand().Id);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_Title_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationUpdateCommand().Title);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_ObservedOn_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(TestObservationUpdateCommand().ObservedOn);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_Latitude_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationUpdateCommand().Latitude);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_Longitude_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationUpdateCommand().Longitude);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_Address_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationUpdateCommand().Address);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_IsIdentificationRequired_Is_TypeOf_Bool()
        {
            Assert.IsInstanceOf<bool>(TestObservationUpdateCommand().IsIdentificationRequired);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_ObservationCategory_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationUpdateCommand().ObservationCategory);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_MediaResources_Is_TypeOf_ListOfStrings()
        {
            Assert.IsInstanceOf<List<string>>(TestObservationUpdateCommand().MediaResources);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_Username_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationUpdateCommand().Username);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationUpdateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<NotImplementedException>(() => new ObservationUpdateCommand().ValidationResults()));
        }

        #endregion 
    }
}