/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class ObservationTest
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

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_Constructor()
        {
            var observation = new Observation(
                FakeObjects.TestUserWithId(),
                FakeValues.Title,
                FakeValues.CreatedDateTime,
                FakeValues.CreatedDateTime,
                FakeValues.Latitude,
                FakeValues.Longitude,
                FakeValues.Address,
                FakeValues.IsTrue,
                FakeValues.Category,
                new List<MediaResource>() {FakeObjects.TestImageMediaResourceWithId()}
                );

            Assert.AreEqual(FakeObjects.TestUserWithId().DenormalisedUserReference(), observation.User);
            Assert.AreEqual(FakeValues.Title, observation.Title);
            Assert.AreEqual(FakeValues.CreatedDateTime, observation.CreatedOn);
            Assert.AreEqual(FakeValues.CreatedDateTime, observation.ObservedOn);
            Assert.AreEqual(FakeValues.Latitude, observation.Latitude);
            Assert.AreEqual(FakeValues.Longitude, observation.Longitude);
            Assert.AreEqual(FakeValues.Address, observation.Address);
            Assert.AreEqual(FakeValues.IsTrue, observation.IsIdentificationRequired);
            Assert.AreEqual(FakeValues.Category, observation.ObservationCategory);
            Assert.AreEqual(FakeValues.Title, observation.Title);
            Assert.IsTrue(observation.MediaResources.ToList().Count == 1);
            Assert.AreEqual(FakeObjects.TestImageMediaResourceWithId(), observation.MediaResources.ToList()[0]);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_UpdateDetails()
        {
            var observation = new Observation(
                FakeObjects.TestUserWithId(),
                FakeValues.Title,
                FakeValues.CreatedDateTime,
                FakeValues.CreatedDateTime,
                FakeValues.Latitude,
                FakeValues.Longitude,
                FakeValues.Address,
                FakeValues.IsTrue,
                FakeValues.Category,
                new List<MediaResource>());

            observation.UpdateDetails(
                FakeObjects.TestUserWithId(),
                FakeValues.Title.PrependWith("new"),
                FakeValues.ModifiedDateTime,
                FakeValues.Latitude.PrependWith("new"),
                FakeValues.Longitude.PrependWith("new"),
                FakeValues.Address.PrependWith("new"),
                FakeValues.IsFalse,
                FakeValues.Category.PrependWith("new"),
                new List<MediaResource>() { FakeObjects.TestImageMediaResourceWithId() }
                );

            Assert.AreEqual(FakeValues.Title.PrependWith("new"), observation.Title);
            Assert.AreEqual(FakeValues.ModifiedDateTime, observation.ObservedOn);
            Assert.AreEqual(FakeValues.Latitude.PrependWith("new"), observation.Latitude);
            Assert.AreEqual(FakeValues.Longitude.PrependWith("new"), observation.Longitude);
            Assert.AreEqual(FakeValues.Address.PrependWith("new"), observation.Address);
            Assert.AreEqual(FakeValues.IsFalse, observation.IsIdentificationRequired);
            Assert.AreEqual(FakeValues.Category.PrependWith("new"), observation.ObservationCategory);
        }

        #endregion
    }
}