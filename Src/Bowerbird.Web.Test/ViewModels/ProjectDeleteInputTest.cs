/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.Test.ViewModels
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ProjectDeleteInputTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize()
        {
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectDeleteInput_ProjectId_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ProjectDeleteInput() { ProjectId = FakeValues.KeyString }.ProjectId);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectDeleteInput_UserId_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new ProjectDeleteInput() { UserId = FakeValues.KeyString }.UserId);
        }

        #endregion

        #region Method tests

        #endregion
    }
}