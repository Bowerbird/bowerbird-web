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

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Extensions;
    using Bowerbird.Core.DomainModels.Posts;

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
        public void ProjectPost_Constructor_Populates_Properties_With_Passed_Values()
        {
            var projectPost = 
                    new ProjectPost(
                        FakeObjects.TestProject(),
                        FakeObjects.TestUser(),
                        FakeValues.CreatedDateTime,
                        FakeValues.Subject,
                        FakeValues.Message,
                        new List<MediaResource>(){new ProxyObjects.ProxyMediaResource(FakeValues.Filename,FakeValues.FileFormat, FakeValues.Description)});

            var id = (new Random(DateTime.Now.Millisecond)).Next().ToString();
            ((IAssignableId)projectPost).SetIdTo("projectpost", id);

            Assert.AreEqual(projectPost.Id, id.PrependWith("projectpost/"));
            Assert.AreEqual(projectPost.Message, FakeValues.Message);
            Assert.AreEqual(projectPost.PostedOn, FakeValues.CreatedDateTime);
            Assert.AreEqual(projectPost.MediaResources[0].OriginalFileName, FakeValues.Filename);
            Assert.AreEqual(projectPost.MediaResources[0].FileFormat, FakeValues.FileFormat);
            Assert.AreEqual(projectPost.MediaResources[0].Description, FakeValues.Description);
        }

        #endregion

        #region Method tests

        #endregion 
    }
}