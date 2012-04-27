/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using NUnit.Framework;
using Bowerbird.Test.Utils;
using Bowerbird.Core.DomainModels;
using System.Collections.Generic;

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
    public class MediaResourceTest
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
        public void MediaResource_Constructor()
        {
            var testMediaResource = new MediaResource(
                "image",
                FakeObjects.TestUser(),
                FakeValues.CreatedDateTime,
                new Dictionary<string, string>() { { "OriginalFileName", FakeValues.Filename } });

            Assert.AreEqual(testMediaResource.Type, "image");
            Assert.AreEqual(testMediaResource.CreatedByUser, FakeObjects.TestUser().DenormalisedUserReference());
            Assert.AreEqual(testMediaResource.UploadedOn, FakeValues.CreatedDateTime);
            Assert.AreEqual(testMediaResource.Metadata, new Dictionary<string, string>() { { "OriginalFileName", FakeValues.Filename } });
        }

        #endregion

        #region Method tests

        #endregion
    }
}