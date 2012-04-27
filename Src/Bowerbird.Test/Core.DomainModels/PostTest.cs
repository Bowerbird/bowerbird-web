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
using NUnit.Framework;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;

namespace Bowerbird.Test.Core.DomainModels
{
    [TestFixture]
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

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Constructor()
        {
            var testPost = new Post(
                FakeObjects.TestUserWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Subject,
                FakeValues.Message,
                new List<MediaResource>() {FakeObjects.TestImageMediaResourceWithId()},
                FakeObjects.TestProjectWithId());

            Assert.AreEqual(testPost.User.Id, FakeObjects.TestUserWithId().Id);
            Assert.AreEqual(testPost.User.FirstName, FakeObjects.TestUserWithId().FirstName);
            Assert.AreEqual(testPost.User.LastName, FakeObjects.TestUserWithId().LastName);
            Assert.AreEqual(testPost.Subject, FakeValues.Subject);
            Assert.AreEqual(testPost.Message, FakeValues.Message);
            Assert.AreEqual(testPost.CreatedOn, FakeValues.CreatedDateTime);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_UpdateDetails()
        {
            var testPost = new Post(
                FakeObjects.TestUserWithId(),
                FakeValues.CreatedDateTime,
                FakeValues.Subject,
                FakeValues.Message,
                new List<MediaResource>(),
                FakeObjects.TestProjectWithId());

            testPost.UpdateDetails(
                FakeObjects.TestUserWithId(),
                FakeValues.Subject.AppendWith(additionalString),
                FakeValues.Message.AppendWith(additionalString),
                new List<MediaResource>() { FakeObjects.TestImageMediaResourceWithId() });

            Assert.AreEqual(testPost.Subject, FakeValues.Subject.AppendWith(additionalString));
            Assert.AreEqual(testPost.Message, FakeValues.Message.AppendWith(additionalString));
            Assert.IsTrue(testPost.MediaResources.ToList().Count == 1);
        }

        #endregion
    }
}