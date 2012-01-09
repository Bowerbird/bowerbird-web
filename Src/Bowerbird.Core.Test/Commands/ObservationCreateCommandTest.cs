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
    public class ObservationCreateCommandTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static ObservationCreateCommand TestObservationCreateCommand()
        {
            return new ObservationCreateCommand()
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
                UserId = FakeValues.UserId
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
        public void ObservationCreateCommand_Id_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Id);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_Title_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Title);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_ObservedOn_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(TestObservationCreateCommand().ObservedOn);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_Latitude_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Latitude);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_Longitude_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Longitude);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_Address_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Address);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_IsIdentificationRequired_Is_TypeOf_Bool()
        {
            Assert.IsInstanceOf<bool>(TestObservationCreateCommand().IsIdentificationRequired);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_ObservationCategory_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().ObservationCategory);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_MediaResources_Is_TypeOf_ListOfStrings()
        {
            Assert.IsInstanceOf<List<string>>(TestObservationCreateCommand().MediaResources);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_UserId_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().UserId);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<NotImplementedException>(() => new ObservationCreateCommand().ValidationResults()));
        }

        #endregion
    }
}