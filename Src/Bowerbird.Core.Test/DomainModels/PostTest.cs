/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DomainModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
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
            return new Post(FakeObjects.TestUser(), FakeValues.Subject, FakeValues.Message);
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Constructor_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Post(null, FakeValues.Subject, FakeValues.Message)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Constructor_Passing_Empty_Subject_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Post(FakeObjects.TestUser(), string.Empty, FakeValues.Message)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Constructor_Passing_Empty_Message_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new Post(FakeObjects.TestUser(), FakeValues.Subject, string.Empty)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Constructor_Populates_Properties_With_Values()
        {
            var testPost = new Post(FakeObjects.TestUser(), FakeValues.Subject, FakeValues.Message);

            Assert.AreEqual(testPost.User.Id, FakeObjects.TestUser().Id);
            Assert.AreEqual(testPost.User.FirstName, FakeObjects.TestUser().FirstName);
            Assert.AreEqual(testPost.User.LastName, FakeObjects.TestUser().LastName);
            Assert.AreEqual(testPost.User.Email, FakeObjects.TestUser().Email);
            Assert.AreEqual(testPost.Subject, FakeValues.Subject);
            Assert.AreEqual(testPost.Message, FakeValues.Message);
            Assert.AreEqual(testPost.PostedOn.Day, DateTime.Now.Day);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_User_Is_OfType_DenormalisedUserReference()
        {
            Assert.IsInstanceOf<DenormalisedUserReference>(TestPost().User);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Subject_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestPost().Subject);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_Message_Is_OfType_String()
        {
            Assert.IsInstanceOf<string>(TestPost().Message);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Post_PostedOn_Is_OfType_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(TestPost().PostedOn);
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
                FakeValues.Message.AppendWith(additionalString));

            Assert.AreEqual(testPost.Subject, FakeValues.Subject.AppendWith(additionalString));
            Assert.AreEqual(testPost.Message, FakeValues.Message.AppendWith(additionalString));
        }

        #endregion
    }
}