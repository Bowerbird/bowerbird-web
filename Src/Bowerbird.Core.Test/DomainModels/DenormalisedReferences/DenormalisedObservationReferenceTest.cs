/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DomainModels.DenormalisedReferences
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class DenormalisedObservationReferenceTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static DenormalisedObservationReference TestDenormalise(Observation observation)
        {
            DenormalisedObservationReference denormalisedObservationReference = observation;

            return denormalisedObservationReference;
        }

        private static Observation TestObservation()
        {
            return new Observation(FakeObjects.TestUser(), FakeValues.Title, FakeValues.CreatedDateTime, FakeValues.Latitude, FakeValues.Longitude, FakeValues.Address, FakeValues.IsTrue, FakeValues.Category, TestMediaResources());
        }

        private static IEnumerable<MediaResource> TestMediaResources()
        {
            return new List<MediaResource>() { new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description) };
        }
        
        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedObservationReference_Implicit_Operator_Passing_Observation_Returns_DenormalisedObservationReference_With_Populated_Properties()
        {
            var normalisedObservation = TestObservation();

            DenormalisedObservationReference denormalisedObservation = TestDenormalise(normalisedObservation);

            Assert.AreEqual(denormalisedObservation.Id, normalisedObservation.Id);
            Assert.AreEqual(denormalisedObservation.Title, normalisedObservation.Title);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedObservationReference_Implicit_Operator_Passing_Null_Observation_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => TestDenormalise(null)));
        }

        #endregion 
    }
}