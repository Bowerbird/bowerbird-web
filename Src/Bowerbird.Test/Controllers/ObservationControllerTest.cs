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

    using Bowerbird.Web;
    using Bowerbird.Web.Controllers.Members;
    using NUnit.Framework;
    using Moq;
    using Bowerbird.Core;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ObservationControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private ObservationController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _controller = new ObservationController(
                _mockCommandProcessor.Object,
                null
                );
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Observation_()
        {

        }

        #endregion
    }
}