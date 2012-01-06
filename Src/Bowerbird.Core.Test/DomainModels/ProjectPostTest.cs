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
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Extensions;

    #endregion

    [TestFixture]
    public class ProjectPostTest
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
        public void ProjectPost_Constructor_Passing_Null_Project_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectPost(
                        null,
                        FakeObjects.TestUser(),
                        FakeValues.Subject,
                        FakeValues.Message
                        )));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPost_Constructor_Populates_Properties_With_Passed_Values()
        {
            var projectPost = 
                    new ProjectPost(
                        FakeObjects.TestProject(),
                        FakeObjects.TestUser(),
                        FakeValues.Subject,
                        FakeValues.Message
                        );

            var id = (new Random(DateTime.Now.Millisecond)).Next().ToString();
            ((IAssignableId)projectPost).SetIdTo("projectpost", id);

            Assert.AreEqual(projectPost.Id, id.PrependWith("projectpost/"));
            Assert.AreEqual(projectPost.Message, FakeValues.Message);
            Assert.AreEqual(projectPost.PostedOn.Day, DateTime.Now.Day);
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        #endregion 
    }
}