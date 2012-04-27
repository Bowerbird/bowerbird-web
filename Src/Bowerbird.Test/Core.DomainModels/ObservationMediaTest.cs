/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
    public class ObservationMediaTest
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
        public void ObservationMedia_Constructor()
        {
            var observationMedia = new ObservationMedia(    
                    FakeValues.KeyString,
                    FakeObjects.TestMediaResourceWithId(),
                    FakeValues.Description,
                    FakeValues.Description
                );

            Assert.AreEqual(FakeValues.KeyString, observationMedia.Id);
            Assert.AreEqual(FakeObjects.TestMediaResourceWithId(), observationMedia.MediaResource);
            Assert.AreEqual(FakeValues.Description, observationMedia.Description);
            Assert.AreEqual(FakeValues.Description, observationMedia.Licence);
        }

        #endregion

        #region Method Tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationMedia_UpdateDetails()
        {
            var observationMedia = new ObservationMedia(
                        FakeValues.KeyString,
                        FakeObjects.TestMediaResourceWithId(),
                        FakeValues.Description,
                        FakeValues.Description
                    );

            observationMedia.UpdateDetails(
                FakeValues.Description.AppendWith("new"),
                FakeValues.Description.AppendWith("new")
                );

            Assert.AreEqual(observationMedia.Description, FakeValues.Description.AppendWith("new"));
            Assert.AreEqual(observationMedia.Licence, FakeValues.Description.AppendWith("new"));
        }

        #endregion
    }
}