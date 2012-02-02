/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.DomainModels
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Extensions;
    using Bowerbird.Test.Utils;

    #endregion

    public class PostTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        const string additionalString = "_";

        private static Post TestPost()
        {
            return new ProxyObjects.ProxyPost(FakeObjects.TestUser(), FakeValues.CreatedDateTime, FakeValues.Subject, FakeValues.Message, new List<MediaResource>());
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Constructor_Populates_Properties_With_Values()
        {
            var testPost = new ProxyObjects.ProxyPost(
                FakeObjects.TestUser(),
                FakeValues.CreatedDateTime,
                FakeValues.Subject,
                FakeValues.Message,
                new List<MediaResource>() {new ProxyObjects.ProxyMediaResource(FakeValues.Filename,FakeValues.FileFormat,FakeValues.Description)});

            Assert.AreEqual(testPost.User.Id, FakeObjects.TestUser().Id);
            Assert.AreEqual(testPost.User.FirstName, FakeObjects.TestUser().FirstName);
            Assert.AreEqual(testPost.User.LastName, FakeObjects.TestUser().LastName);
            Assert.AreEqual(testPost.Subject, FakeValues.Subject);
            Assert.AreEqual(testPost.Message, FakeValues.Message);
            Assert.AreEqual(testPost.CreatedOn, FakeValues.CreatedDateTime);
            Assert.AreEqual(testPost.MediaResources[0].Description, FakeValues.Description);
            Assert.AreEqual(testPost.MediaResources[0].OriginalFileName, FakeValues.Filename);
            Assert.AreEqual(testPost.MediaResources[0].FileFormat, FakeValues.FileFormat);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_UpdateDetails_Populates_Properties_With_Values()
        {
            var testPost = TestPost();

            testPost.UpdateDetails(
                FakeObjects.TestUser(),
                FakeValues.Subject.AppendWith(additionalString),
                FakeValues.Message.AppendWith(additionalString),
                new List<MediaResource>(){new ProxyObjects.ProxyMediaResource(FakeValues.Filename,FakeValues.FileFormat,FakeValues.Description)});

            Assert.AreEqual(testPost.Subject, FakeValues.Subject.AppendWith(additionalString));
            Assert.AreEqual(testPost.Message, FakeValues.Message.AppendWith(additionalString));
            Assert.AreEqual(testPost.MediaResources[0].Description, FakeValues.Description);
            Assert.AreEqual(testPost.MediaResources[0].FileFormat, FakeValues.FileFormat);
            Assert.AreEqual(testPost.MediaResources[0].OriginalFileName, FakeValues.Filename);
        }

        #endregion
    }
}