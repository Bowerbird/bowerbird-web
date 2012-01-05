/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Test.CommandFactories
{
    #region Namespaces

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;
    using Bowerbird.Web.ViewModels;
    using NUnit.Framework;
    using Bowerbird.Web.CommandFactories;

    #endregion

    [TestFixture]
    public class ObservationCreateCommandFactoryTest
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

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)]
        public void ObservationCreateCommandFactory_Make_Passing_Null_ObservationCreateInput_ThrowsDesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationCreateCommandFactory().Make(null)));
        }

        [Test, Category(TestCategory.Unit)]
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

        #endregion 

    }

}