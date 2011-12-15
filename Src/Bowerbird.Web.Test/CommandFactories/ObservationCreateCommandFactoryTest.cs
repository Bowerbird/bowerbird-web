using Bowerbird.Core.DesignByContract;
using Bowerbird.Test.Utils;
using Bowerbird.Web.ViewModels;
using NUnit.Framework;
using Bowerbird.Web.CommandFactories;

namespace Bowerbird.Web.Test.CommandFactories
{

    [TestFixture]
    public class ObservationCreateCommandFactoryTest
    {
        
        [Test]
        public void ObservationCreateCommandFactory_Make_Passing_Null_ObservationCreateInput_ThrowsDesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new ObservationCreateCommandFactory().Make(null)));
        }

        [Test]
        public void ObservationCreateCommandFactory_Make_Passing_ObservationCreateInput_Returns_Populated_ObservationCreateCommand()
        {
            var observationCreateCommandFactory = new ObservationCreateCommandFactory();

            var observationCreateInput = new ObservationCreateInput()
            {
                Title = FakeValues.Title,
                Latitude = FakeValues.Latitude,
                Longitude = FakeValues.Longitude,
                Address = FakeValues.Address,
                IsIdentificationRequired = FakeValues.IsTrue,
                MediaResources = FakeValues.StringList,
                ObservationCategory = FakeValues.Category,
                ObservedOn = FakeValues.CreatedDateTime,
                Username = FakeValues.UserName
            };

            var observationCreateCommand = observationCreateCommandFactory.Make(observationCreateInput);

            Assert.AreEqual(observationCreateInput.Title, observationCreateCommand.Title);
            Assert.AreEqual(observationCreateInput.Latitude, observationCreateCommand.Latitude);
            Assert.AreEqual(observationCreateInput.Longitude, observationCreateCommand.Longitude);
            Assert.AreEqual(observationCreateInput.Address, observationCreateCommand.Address);
            Assert.AreEqual(observationCreateInput.IsIdentificationRequired, observationCreateCommand.IsIdentificationRequired);
            Assert.AreEqual(observationCreateInput.MediaResources, observationCreateCommand.MediaResources);
            Assert.AreEqual(observationCreateInput.ObservationCategory, observationCreateCommand.ObservationCategory);
            Assert.AreEqual(observationCreateInput.ObservedOn, observationCreateCommand.ObservedOn);
            Assert.AreEqual(observationCreateInput.Username, observationCreateCommand.Username);
        }
    }

}