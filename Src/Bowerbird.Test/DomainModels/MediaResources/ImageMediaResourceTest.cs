/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Extensions;
using NUnit.Framework;
using Bowerbird.Test.Utils;
using Bowerbird.Core.DomainModels.MediaResources;

namespace Bowerbird.Test.DomainModels.MediaResources
{
    [TestFixture]
    public class ImageMediaResourceTest
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
        public void ImageMediaResource_Constructor()
        {
            var testMediaResource = new ImageMediaResource(
                FakeObjects.TestUser(),
                FakeValues.CreatedDateTime, 
                FakeValues.Filename, 
                FakeValues.FileFormat, 
                FakeValues.Description, 
                FakeValues.Number, 
                FakeValues.Number);

            Assert.AreEqual(testMediaResource.OriginalFileName, FakeValues.Filename);
            Assert.AreEqual(testMediaResource.FileFormat, FakeValues.FileFormat);
            Assert.AreEqual(testMediaResource.Description, FakeValues.Description);
            Assert.AreEqual(testMediaResource.Width, FakeValues.Number);
            Assert.AreEqual(testMediaResource.Height, FakeValues.Number);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResource_UpdateDetails()
        {
            ImageMediaResource imageMediaResource = FakeObjects.TestImageMediaResourceWithId() as ImageMediaResource;

            imageMediaResource
                .UpdateDetails(FakeValues.Description.PrependWith("new"));

            Assert.AreEqual(imageMediaResource.Description, FakeValues.Description.PrependWith("new"));
        }

        #endregion
    }
}