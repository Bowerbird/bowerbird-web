/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.Controllers
{
    #region Namespaces

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;
    using Bowerbird.Web.Controllers;

    #endregion

    [TestFixture]
    public class ProjectMemberControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private ProjectMemberController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();

            _controller = new ProjectMemberController(_mockCommandProcessor.Object);
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMember_Constructor_With_Null_CommandProcessor_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectMemberController(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Integration)]
        public void ProjectMember_List()
        {

        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ProjectMember_Create()
        {

        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ProjectMember_Delete()
        {

        }

        #endregion
    }
}