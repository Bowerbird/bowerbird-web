using System;
using System.Collections.Generic;
using Bowerbird.Core.Commands;
using NUnit.Framework;

namespace Bowerbird.Core.Test.Commands
{
    [TestFixture]
    public class ObservationCreateCommandTest
    {

        #region Constructor tests


        #endregion

        #region Property tests

        [Test]
        public void ObservationCreateCommand_Id_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Id);
        }

        [Test]
        public void ObservationCreateCommand_Title_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Title);
        }

        [Test]
        public void ObservationCreateCommand_ObservedOn_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(TestObservationCreateCommand().ObservedOn);
        }

        [Test]
        public void ObservationCreateCommand_Latitude_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Latitude);
        }

        [Test]
        public void ObservationCreateCommand_Longitude_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Longitude);
        }

        [Test]
        public void ObservationCreateCommand_Address_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Address);
        }

        [Test]
        public void ObservationCreateCommand_IsIdentificationRequired_Is_TypeOf_Bool()
        {
            Assert.IsInstanceOf<bool>(TestObservationCreateCommand().IsIdentificationRequired);
        }

        [Test]
        public void ObservationCreateCommand_ObservationCategory_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().ObservationCategory);
        }

        [Test]
        public void ObservationCreateCommand_MediaResources_Is_TypeOf_ListOfStrings()
        {
            Assert.IsInstanceOf<List<string>>(TestObservationCreateCommand().MediaResources);
        }

        [Test]
        public void ObservationCreateCommand_Username_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestObservationCreateCommand().Username);
        }

        #endregion

        #region Method tests

        [Test, Ignore]
        public void ObservationCreateCommand_ValidationResults_WithInvalidInput_RaisesValidationError()
        {
            // TODO: Complete test
        }

        #endregion

        #region Helpers

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
                Username = FakeValues.UserName
            };
        }

        private static List<string> TestMediaResources()
        {
            return new List<string>() { "mediaresource/123", "mediaresource/124", "mediaresource/125" };
        }

        #endregion
				
    }
}